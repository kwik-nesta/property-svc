using System.ComponentModel.DataAnnotations;

namespace KwikNesta.Property.Svc.Domain.Models
{
    public class PropertyLocation : BaseEntity
    {
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
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Navigation
        public ICollection<RealEstateProperty> Properties { get; set; } = [];
    }
}