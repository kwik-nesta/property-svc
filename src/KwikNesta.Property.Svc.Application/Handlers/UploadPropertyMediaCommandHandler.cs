using API.Common.Response.Model.Exceptions;
using Cloudtenary.Abstract;
using CrossQueue.Hub.Services.Interfaces;
using CSharpTypes.Extensions.Enumeration;
using CSharpTypes.Extensions.Guid;
using Hangfire;
using KwikNesta.Contracts.Commands;
using KwikNesta.Contracts.Enums;
using KwikNesta.Contracts.Models;
using KwikNesta.Mediatrix.Core.Abstractions;
using KwikNesta.Property.Svc.Application.Commands;
using KwikNesta.Property.Svc.Application.Common.Extensions;
using KwikNesta.Property.Svc.Application.Common.Interfaces;
using KwikNesta.Property.Svc.Application.Common.Models.Enums;
using KwikNesta.Property.Svc.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace KwikNesta.Property.Svc.Application.Handlers
{
    public class UploadPropertyMediaCommandHandler : IKwikRequestHandler<UploadPropertyMediaCommand, ApiResult<string>>
    {
        private readonly IRepositoryManager _repository;
        private readonly IRabbitMQPubSub _rabbitMQ;
        private readonly ICloudtenary _cloudtenary;
        private readonly ClaimsPrincipal? _claim;

        public UploadPropertyMediaCommandHandler(IRepositoryManager repository,
                                                 IRabbitMQPubSub rabbitMQ,
                                                 IHttpContextAccessor accessor,
                                                 ICloudtenary cloudtenary)
        {
            _repository = repository;
            _rabbitMQ = rabbitMQ;
            _cloudtenary = cloudtenary;
            _claim = accessor.HttpContext?.User;
        }

        public async Task<ApiResult<string>> HandleAsync(UploadPropertyMediaCommand request, CancellationToken cancellationToken)
        {
            var userId = _claim?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new ForbiddenException("Access denied. Loggedin user not found");
            }

            if (request == null || request.PropertyId.IsEmpty())
            {
                throw new BadRequestException("Invalid request");
            }

            if (request.Images.Count == 0 && request.Video == null)
            {
                throw new BadRequestException("Please select one or more files to upload");
            }

            var property = await _repository
                .Property.GetAsync(request.PropertyId, true) ??
                throw new NotFoundException("No property record found for this Id");

            if(property.Media.Count > 0)
            {
                var imageCount = property.Media.Count(p => p.Type == MediaType.Image);
                if(imageCount + request.Images.Count > 5)
                {
                    throw new BadRequestException($"You already have {imageCount} images uploaded for this property. Maximum upload per property is 5. Please select fewer images to upload or delete some to continue.");
                }
                if(property.Media.Any(m => m.Type == MediaType.Video) && request.Video != null)
                {
                    throw new BadRequestException("You've reached the maximum number of video upload allowed per property.");
                }
            }

            var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            request.Images.ForEach(img =>
            {
                var (Valid, Message) = img.IsAValidFile(UploadMediaType.Image);
                if (!Valid)
                {
                    throw new BadRequestException(Message);
                }
            });

            if(request.Video != null)
            {
                var (Valid, Message) = request.Video.IsAValidFile(UploadMediaType.Video);
                if (!Valid)
                {
                    throw new BadRequestException(Message);
                }
            }

            var imageFilePaths = new List<string>();
            foreach (var image in request.Images)
            {
                var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
                var fileName = Helpers.GetFileName(property.Id, fileExtension);
                var filePath = Path.Combine(uploadDirectory, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(stream, cancellationToken);
                imageFilePaths.Add(filePath);
            }

            var videoFilePath = string.Empty;
            if(request.Video != null)
            {
                var videoExtension = Path.GetExtension(request.Video.FileName).ToLowerInvariant();
                var videoFileName = Helpers.GetFileName(property.Id, videoExtension);
                var videoPath = Path.Combine(uploadDirectory, videoFileName);
                using var stream = new FileStream(videoPath, FileMode.Create);
                await request.Video.CopyToAsync(stream, cancellationToken);
                videoFilePath = videoPath;
            }

            BackgroundJob.Enqueue<IUploadService>(u 
                => u.UploadPropertyMediaAsync(imageFilePaths, videoFilePath, property.Id, null!));
            property.IsLocked = true;
            property.LastUpdatedOn = DateTime.UtcNow;
            await _repository.Property
                .UpdateAsync(property);
            return new ApiResult<string>("Media files are being uploaded. You'll be notified when complete.");
        }
    }
}