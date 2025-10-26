using API.Common.Response.Model.Exceptions;
using CrossQueue.Hub.Services.Interfaces;
using CSharpTypes.Extensions.Enumeration;
using KwikNesta.Contracts.Commands;
using KwikNesta.Contracts.Enums;
using KwikNesta.Contracts.Models;
using KwikNesta.Mediatrix.Core.Abstractions;
using KwikNesta.Property.Svc.Application.Commands;
using KwikNesta.Property.Svc.Application.Common.Extensions;
using KwikNesta.Property.Svc.Application.Common.Interfaces;
using KwikNesta.Property.Svc.Application.Dtos;
using KwikNesta.Property.Svc.Application.Validations;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace KwikNesta.Property.Svc.Application.Handlers
{
    public class AddPropertyCommandHandler : IKwikRequestHandler<AddPropertyCommand, ApiResult<AddPropertyDto>>
    {
        private readonly IRepositoryManager _repository;
        private readonly IRabbitMQPubSub _rabbitMQ;
        private readonly ILocationClientService _location;
        private readonly ClaimsPrincipal? _claim;

        public AddPropertyCommandHandler(IRepositoryManager repository,
                                         IRabbitMQPubSub rabbitMQ,
                                         IHttpContextAccessor contextAccessor,
                                         ILocationClientService location)
        {
            _repository = repository;
            _rabbitMQ = rabbitMQ;
            _location = location;
            _claim = contextAccessor.HttpContext?.User;
        }

        public async Task<ApiResult<AddPropertyDto>> HandleAsync(AddPropertyCommand request, CancellationToken cancellationToken)
        {
            var userId = _claim?.FindFirstValue(ClaimTypes.NameIdentifier);
            if(string.IsNullOrEmpty(userId))
            {
                throw new ForbiddenException("Access denied. Loggedin user not found");
            }

            var validator = new AddPropertyCommandValidator()
                .Validate(request);
            if (!validator.IsValid)
            {
                throw new BadRequestException(validator.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid input parameters.");
            }

            var countryResponse = await _location.GetCountryAsyncV1(request.Location!.CountryId);
            if (!countryResponse.IsSuccessStatusCode || countryResponse.Content == null)
            {
                throw new NotFoundException(countryResponse.Error?.Message ?? "There was an error getting location data.");
            }

            var country = countryResponse.Content;
            var state = country.States.FirstOrDefault(s => s.CountryId == country.Id);
            if (string.IsNullOrWhiteSpace(request.Location?.Latitude) || 
                string.IsNullOrWhiteSpace(request.Location?.Longitude))
            {
                request.Location!.Longitude = state?.Longitude;
                request.Location!.Latitude = state?.Latitude;
            }
            var property = request.Map(userId, country.Name, state?.Name ?? "");
            await _repository.Property.CreateAsync(property);

            await _rabbitMQ.PublishAsync(new AuditCommand
            {
                Domain = AuditDomain.Property,
                DomainId = property.Id,
                Action = AuditAction.PropertyCreated,
                PerformedBy = userId,
                TargetId = property.Id.ToString()
            }, routingKey: MQRoutingKey.AuditTrails.GetDescription());
            return new ApiResult<AddPropertyDto>(new AddPropertyDto(property.Id), "Property successfully added");
        }
    }
}