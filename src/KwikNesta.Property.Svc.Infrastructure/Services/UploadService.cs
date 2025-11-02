using Cloudtenary.Abstract;
using Cloudtenary.Models;
using CrossQueue.Hub.Services.Interfaces;
using CSharpTypes.Extensions.Enumeration;
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
    public class UploadService : IUploadService
    {
        private readonly ICloudtenary _cloudtenary;
        private readonly IRepositoryManager _repository;
        private readonly IRabbitMQPubSub _rabbitMQ;

        public UploadService(ICloudtenary cloudtenary,
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
                        overlayText: $"© {DateTime.UtcNow.Year} Kwik Nesta"));
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
                            overlayText: $"© {DateTime.UtcNow.Year} Kwik Nesta"));
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
    }
}
