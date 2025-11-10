using CSharpTypes.Extensions.Enumeration;
using KwikNesta.Property.Svc.Application.Commands;
using KwikNesta.Property.Svc.Application.Common.Models.Enums;
using KwikNesta.Property.Svc.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace KwikNesta.Property.Svc.Application.Common.Extensions
{
    public static class Helpers
    {
        private const long MaxImageSize = 2048000; //2mb
        private const long MaxDocSize = 1024000; //1mb
        private const long MaxVideoSize = 1024000000;

        public static bool IsValidLocation(this PropertyLocationRequest? location)
        {
            return location != null && 
                !string.IsNullOrEmpty(location.Address) && 
                !string.IsNullOrEmpty(location.City) && 
                //!string.IsNullOrEmpty(location.PostalCode) && 
                location.CountryId != Guid.Empty && location.StateId != Guid.Empty;
        }

        public static RealEstateProperty Map(this AddPropertyCommand command, 
                                             string userId, string country, 
                                             string state, bool cordinateSent)
        {
            var property = new RealEstateProperty
            {
                OwnerId = userId,
                Title = command.Title,
                Description = command.Description,
                Price = command.Price,
                Currency = command.Currency,
                Type = command.Type,
                IsCoordinatesSent = cordinateSent,
            };

            property.Location = new PropertyLocation
            {
                AddressLine = command.Location!.Address,
                City = command.Location!.City,
                Country = country,
                State = state,
                Longitude = command.Location!.Longitude,
                Latitude = command.Location!.Latitude,
                PostalCode = command.Location!.PostalCode 
            };

            property.Feature = new PropertyFeature
            {
                AreaSize = command.Features!.AreaSize,
                AreaUnit = command.Features!.AreaSizeUnit,
                Bathrooms = command.Features!.Bathrooms,
                HasElectricity = command.Features!.HasElectricity,
                HasParking = command.Features!.HasParking,
                HasWaterSupply = command.Features!.HasWaterSupply,
                Bedrooms = command.Features!.Bedrooms
            };

            return property;
        }

        public static (bool Valid, string Message) IsAValidFile(this IFormFile file, UploadMediaType mediaType)
        {
            if (!Enum.IsDefined(typeof(UploadMediaType), mediaType))
                return (false, "Invalid media type");

            if (file is null || file.Length <= 0)
                return (false, "Please upload a file");

            if (mediaType == UploadMediaType.Image)
            {
                var allowedFormats = mediaType.GetDescription().Split('|');
                if (!allowedFormats.Any(f => file.FileName.EndsWith(f)))
                    return (false, string.Format("Invalid image type. Accepted extensions: {0}", string.Join(',', allowedFormats)));

                if (file.Length > MaxImageSize)
                    return (false, $"File size too large. Max image size: 2mb");
            }
            else if(mediaType == UploadMediaType.Video)
            {
                var allowedFormats = mediaType.GetDescription().Split('|');
                if (!allowedFormats.Any(f => file.FileName.EndsWith(f)))
                    return (false, string.Format("Invalid image type. Accepted extensions: {0}", string.Join(',', allowedFormats)));

                if (file.Length > MaxVideoSize)
                    return (false, $"File size too large. Max vidoe size: 1gb");
            }
            else if (mediaType == UploadMediaType.Document)
            {
                var allowedFormats = mediaType.GetDescription().Split('|');
                if (!allowedFormats.Any(f => file.FileName.EndsWith(f)))
                    return (false, string.Format("Invalid document type. Accepted extensions: {0}", string.Join(',', allowedFormats)));

                if (file.Length > MaxDocSize)
                    return (false, $"File size too large. Max doc size: 1mb");
            }

            return (true, "Valid");
        }

        public static string GetFileName(Guid entityId, string fileExtension)
        {
            var randomPart = Path.GetRandomFileName().Replace(".", "");
            return $"{entityId:N}-{randomPart}{fileExtension}";
        }
    }
}