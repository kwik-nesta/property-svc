using Cloudtenary.Abstract;
using Cloudtenary.Models;
using CrossQueue.Hub.Services.Interfaces;
using CSharpTypes.Extensions.Enumeration;
using CSharpTypes.Extensions.List;
using CSharpTypes.Extensions.Object;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using KwikNesta.Contracts.Commands;
using KwikNesta.Contracts.Enums;
using KwikNesta.Property.Svc.Application.Common.Interfaces;
using KwikNesta.Property.Svc.Domain.Enums;
using KwikNesta.Property.Svc.Domain.Models;

namespace KwikNesta.Property.Svc.Infrastructure.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly ICloudtenary _cloudtenary;
        private readonly IRepositoryManager _repository;
        private readonly IRabbitMQPubSub _rabbitMQ;

        private static readonly string[] StockSources = 
            { "shutterstock", "pexels", "unsplash", "istockphoto", "gettyimages" };

        public PropertyService(ICloudtenary cloudtenary,
                             IRepositoryManager repository,
                             IRabbitMQPubSub rabbitMQ)
        {
            _cloudtenary = cloudtenary;
            _repository = repository;
            _rabbitMQ = rabbitMQ;
        }

        public async Task UploadPropertyMediaAsync(List<string> imageFilePaths,
                                                   string videoFilePath, 
                                                   Guid propertyId, 
                                                   PerformContext context)
        {
            var openStreams = new List<MemoryStream>();

            try
            {
                context.WriteLine("Media upload started for property: {0}", propertyId);
                if (imageFilePaths.Count == 0 && string.IsNullOrEmpty(videoFilePath))
                {
                    context.WriteLine("No video or image file to process. Aborting...");
                    return;
                }

                var property = await _repository.Property
                    .GetAsync(propertyId);
                if (property == null)
                {
                    context.WriteLine("No record found for Property: {0}", propertyId);
                    return;
                }
                var existingMedia = await _repository.PropertyMedia
                   .GetRangeAsync(property.Id);

                var existingImages = existingMedia
                    .Where(p => p.Type == MediaType.Image)
                    .ToList();
                var existingVideo = existingMedia
                    .FirstOrDefault(p => p.Type == MediaType.Video);

                var mediaToAdd = new List<PropertyMedia>();
                var uploadTasks = new List<Task<CloudtenaryUploadResult?>>();
                foreach (var imageFilePath in imageFilePaths.Take(5 - existingImages.Count))
                {
                    context.WriteLine($"Reading file content from path: {imageFilePath}");
                    byte[] bytes = File.ReadAllBytes(imageFilePath);
                    var imageStream = new MemoryStream(bytes);
                    openStreams.Add(imageStream);
                    if (imageStream.Length <= 0)
                    {
                        context.WriteLine("Invalid stream length for: {0}", imageFilePath);
                        continue;
                    }

                    imageStream.Position = 0;
                    uploadTasks.Add(_cloudtenary.UploadImageAsync(Path.GetFileName(imageFilePath),
                        Path.GetFileName(imageFilePath),
                        imageStream,
                        overlayText: $"Kwik Nesta"));
                }

                if (!string.IsNullOrEmpty(videoFilePath) && existingVideo == null)
                {
                    byte[] bytes = File.ReadAllBytes(videoFilePath);
                    var videoStream = new MemoryStream(bytes);
                    openStreams.Add(videoStream);
                    if (videoStream.Length > 0)
                    {
                        videoStream.Position = 0;
                        uploadTasks.Add(_cloudtenary.UploadVideoAsync(Path.GetFileName(videoFilePath),
                            videoStream,
                            overlayText: $"Kwik Nesta"));
                    }
                    else
                    {
                        context.WriteLine("Invalid video file. Size: {0} bytes", videoStream.Length);
                    }
                }
                else
                {
                    if (existingVideo != null)
                    {
                        context.WriteLine("A video file already uploaded for this property");
                    }
                    else
                    {
                        context.WriteLine("No video file passed as argument.");
                    }
                }

                var results = await Task.WhenAll(uploadTasks);
                var imageResults = results.Where(r => r != null && !r.Url.EndsWith(".mp4"))
                    .ToList();
                var videoResult = results.FirstOrDefault(r => r?.Url.EndsWith(".mp4") ?? false);

                if (imageResults.Count > 0)
                {
                    imageResults.ForEach(image =>
                    {
                        if (image != null)
                        {
                            var hasCover = existingImages.Any(m => m.IsCover) || mediaToAdd.Any(m => m.IsCover);
                            mediaToAdd.Add(new PropertyMedia
                            {
                                PropertyId = property.Id,
                                Url = image.Url,
                                PublicId = image.PublicId,
                                Type = MediaType.Image,
                                IsCover = !hasCover 
                            });
                        }
                    });
                }

                if (videoResult != null)
                {
                    mediaToAdd.Add(new PropertyMedia
                    {
                        PropertyId = property.Id,
                        Url = videoResult.Url,
                        PublicId = videoResult.PublicId,
                        Type = MediaType.Video
                    });
                }

                context.WriteLine("Adding {0} images and {1} video files for {2}",
                    imageResults.Count,
                    videoResult != null ? 1 : 0,
                    property.Id);

                property.LastUpdatedOn = DateTime.UtcNow;
                property.Status = ListingStatus.Pending;
                await _repository.PropertyMedia
                    .AddRangeAsync(mediaToAdd);

                context.WriteLine("Added {0} images and {1} video files for {2}",
                    imageResults.Count,
                    videoResult != null ? 1 : 0,
                    property.Id);

                await _rabbitMQ.PublishAsync(new AuditCommand
                {
                    Domain = AuditDomain.Property,
                    DomainId = property.Id,
                    Action = AuditAction.PropertyUpdated,
                    Description = "Media files uploaded",
                    PerformedBy = property.OwnerId,
                    TargetId = property.Id.ToString()
                }, routingKey: MQRoutingKey.AuditTrails.GetDescription());
            }
            catch (Exception ex)
            {
                context.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                // Now dispose safely after all uploads finish
                foreach (var stream in openStreams)
                    stream.Dispose();

                imageFilePaths.ForEach(file =>
                {
                    if (File.Exists(file))
                    {
                        context.WriteLine("Cleaning up {0} from memory.", file);

                        File.Delete(file);
                        context.WriteLine("File {0}, deleted successfully", file);
                    }
                });

                if (File.Exists(videoFilePath))
                {
                    context.WriteLine("Cleaning up {0} from memory.", videoFilePath);

                    File.Delete(videoFilePath);
                    context.WriteLine("File {0}, deleted successfully", videoFilePath);
                }
            }
        }

        public async Task VerifyPropertyOwnerShip(Guid id, string userId, PerformContext context)
        {
            context.WriteLine("Property ownership verification started...");

            var property = await _repository.Property
                .GetAsync(id);
            if (property == null)
            {
                context.WriteLine("Property ownership verification failed. Property not found");
                return;
            }

            var (Verified, Reasons) = await VerifyAsync(id);
            var hasDocument = property.Media.Any(p => p.Type == MediaType.Document);
            if (Verified && hasDocument)
            {
                property.IsOwnerShipVerified = true;
                property.LastUpdatedOn = DateTime.UtcNow;
                property.IsCoordinatesSent = true;
                await _repository.Property
                    .UpdateAsync(property);
                context.WriteLine("Property ownership successfully verified.");

                //TODO: Notify the Owner
            }
            else
            {
                property.IsOwnerShipVerified = false;
                property.LastUpdatedOn = DateTime.UtcNow;
                property.VerificationReasons = Reasons;

                await _repository.Property
                    .UpdateAsync(property);
                context.WriteLine("Property ownership verification failed. Reasons: {0}", Reasons);
                //TODO: Notify the owner
            }

            await _rabbitMQ.PublishAsync(new AuditCommand
            {
                Domain = AuditDomain.Property,
                DomainId = property.Id,
                Action = AuditAction.PropertyUpdated,
                Description = "Property ownership verification",
                PerformedBy = userId,
                TargetId = property.Id.ToString()
            }, routingKey: MQRoutingKey.AuditTrails.GetDescription());
        }

        public async Task VerifyPropertyDetails(Guid id, PerformContext context)
        {
            context.WriteLine("Property details verification started...");

            var property = await _repository.Property
                .GetAsync(id);
            if (property == null)
            {
                context.WriteLine("Property details verification failed. Property not found");
                return;
            }

            var (Verified, Reasons) = await VerifyAsync(id);
            if (Verified)
            {
                property.Status = ListingStatus.Available;
                property.LastUpdatedOn = DateTime.UtcNow;
                property.IsCoordinatesSent = true;
                property.IsLocked = false;

                await _repository.Property.UpdateAsync(property);
                context.WriteLine("Property details successfully verified.");

                //TODO: Notify the Owner
            }
            else
            {
                property.Status = ListingStatus.VerificationFailed;
                property.LastUpdatedOn = DateTime.UtcNow;
                property.VerificationReasons = Reasons;
                property.IsLocked = false;
                if (!Verified)
                {
                    property.Status = ListingStatus.VerificationFailed;
                }

                await _repository.Property.UpdateAsync(property);
                context.WriteLine("Property details verification failed. Reasons: {0}", Reasons);
                //TODO: Notify the owner
            }

            await _rabbitMQ.PublishAsync(new AuditCommand
            {
                Domain = AuditDomain.Property,
                DomainId = property.Id,
                Action = AuditAction.PropertyUpdated,
                Description = "Property details verification",
                PerformedBy = property.OwnerId,
                TargetId = property.Id.ToString()
            }, routingKey: MQRoutingKey.AuditTrails.GetDescription());
        }

        public bool IsStockImageAsync(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return true;

            return StockSources.Any(source =>
                imageUrl.Contains(source, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<(bool Verified, string? Reasons)> VerifyAsync(Guid propertyId)
        {
            var reasons = string.Empty;
            var property = await _repository.Property
                .GetAsync(propertyId, true);

            if (property == null)
            {
                return (false, "Property not found");
            }

            var reasonList = new List<string>();
            var hasAllDetails = HasRequiredFields(property);
            var hasMedia = HasEnoughMediaFiles(property.Media);
            var hasValidLocation = HasValidLocation(property.IsCoordinatesSent, property.Location);
            if (hasAllDetails && hasMedia && hasValidLocation)
            {
                return (true, reasons);
            }
            else if(!hasAllDetails)
            {
                reasonList.Add("Property has some missing details.");
            }
            else if(!hasMedia)
            {
                reasonList.Add("Property does not have the required number of images. Minimum is 3.");
            }
            else if (!hasValidLocation)
            {
                reasonList.Add("Location not verified — coordinates missing or inaccurate.");
            }

            return (true, string.Join(" ,", reasonList));
        }

        private static bool HasRequiredFields(RealEstateProperty property)
        {
            return !string.IsNullOrWhiteSpace(property.Title) && 
                !string.IsNullOrWhiteSpace(property.Description) && 
                !string.IsNullOrWhiteSpace(property.Currency) && 
                property.IsCoordinatesSent && property.Price > 0.0m && 
                property.Media.IsNotNullOrEmpty() && property.Location.IsNotNull();
        }

        private static bool HasEnoughMediaFiles(ICollection<PropertyMedia> media)
        {
            return media != null && media
                .Where(m => m.Type == MediaType.Image)
                .ToList().Count >= 3;
        }

        private static bool HasValidLocation(bool coordinateSent, PropertyLocation location)
        {
            var resultAddress = string.Empty;
            // AddressLine, City, State, Country, PostalCode, Verify Coord and city is equal with provided city, and address keywords match
            var isVerified = coordinateSent && location != null &&
                !string.IsNullOrWhiteSpace(location.AddressLine) && 
                !string.IsNullOrWhiteSpace(location.City) && 
                !string.IsNullOrWhiteSpace(location.State) && 
                !string.IsNullOrWhiteSpace(location.Country) && 
                !string.IsNullOrWhiteSpace(location.PostalCode) && 
                resultAddress.Contains(location.City);

            if(isVerified && location != null)
            {
                location.IsVerified = true;
                location.LastUpdatedOn = DateTime.UtcNow;
                //TODO: Save location
            }

            return isVerified;
        }
    }
}
