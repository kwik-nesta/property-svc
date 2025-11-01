using API.Common.Response.Model.Exceptions;
using CrossQueue.Hub.Services.Interfaces;
using CSharpTypes.Extensions.Enumeration;
using CSharpTypes.Extensions.Guid;
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

namespace KwikNesta.Property.Svc.Application.Handlers
{
    public class UploadPropertyMediaCommandHandler : IKwikRequestHandler<UploadPropertyMediaCommand, ApiResult<string>>
    {
        private readonly IRepositoryManager _repository;
        private readonly IRabbitMQPubSub _rabbitMQ;
        private readonly ClaimsPrincipal? _claim;

        public UploadPropertyMediaCommandHandler(IRepositoryManager repository,
                                                 IRabbitMQPubSub rabbitMQ,
                                                 IHttpContextAccessor accessor)
        {
            _repository = repository;
            _rabbitMQ = rabbitMQ;
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

            if(request.Images == null || request.Images.Count == 0)
            {
                throw new BadRequestException("Please select one or more images to upload");
            }

            var property = await _repository
                .Property.GetAsync(request.PropertyId) ??
                throw new NotFoundException("No property record found for this Id");

            foreach (var image in request.Images)
            {
                var validator = image.IsAValidFile(UploadMediaType.Image);
                if (!validator.Valid)
                {
                    throw new BadRequestException(validator.Message);
                }
            }

            if(request.Video != null)
            {
                var videoValidator = request.Video.IsAValidFile(UploadMediaType.Image);
                if (!videoValidator.Valid)
                {
                    throw new BadRequestException(videoValidator.Message);
                }
            }

            //TODO: Upload the images and the video

            property.LastUpdatedOn = DateTime.UtcNow;
            property.Status = ListingStatus.Pending;
            await _repository.Property
                .UpdateAsync(property);

            await _rabbitMQ.PublishAsync(new AuditCommand
            {
                Domain = AuditDomain.Property,
                DomainId = property.Id,
                Action = AuditAction.PropertyUpdated,
                Description = "Media files uploaded",
                PerformedBy = userId,
                TargetId = property.Id.ToString()
            }, routingKey: MQRoutingKey.AuditTrails.GetDescription());

            return new ApiResult<string>("Media files successfully uploaded.");
        }
    }
}