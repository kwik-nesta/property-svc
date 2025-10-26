using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KwikNesta.Property.Svc.Domain.Models
{
    public class PropertyLocation : BaseEntity
    {
        [Required]
        public Guid PropertyId { get; set; }
        [ForeignKey(nameof(PropertyId))]
        public RealEstateProperty Property { get; set; } = default!;

        [Required, MaxLength(100)]
        public string AddressLine { get; set; } = string.Empty;
        [Required, StringLength(100)]
        public string City { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string State { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string Country { get; set; } = "Nigeria";
        [Required, MaxLength(100)]
        public string PostalCode { get; set; } = string.Empty;
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
    }
}