using KwikNesta.Contracts.Models;
using KwikNesta.Mediatrix.Core.Abstractions;
using KwikNesta.Property.Svc.Application.Dtos;
using KwikNesta.Property.Svc.Domain.Enums;

namespace KwikNesta.Property.Svc.Application.Commands
{
    public class AddPropertyCommand : IKwikRequest<ApiResult<AddPropertyDto>>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = "NGN";
        public PropertyType Type { get; set; }
        public PropertyLocationRequest? Location { get; set; }
        public PropertyFeaturesRequest? Features { get; set; }
    }
}
