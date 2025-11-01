using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KwikNesta.Property.Svc.Domain.Models
{
    public class PropertyFeature : BaseEntity
    {
        [Required]
        public Guid PropertyId { get; set; }
        [ForeignKey(nameof(PropertyId))]
        public RealEstateProperty Property { get; set; } = default!;

        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public double AreaSize { get; set; }
        [MaxLength(10)]
        public string AreaUnit { get; set; } = "sqm";
        public bool HasParking { get; set; }
        public bool HasWaterSupply { get; set; }
        public bool HasElectricity { get; set; }
    }
}