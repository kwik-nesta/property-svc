using System.ComponentModel.DataAnnotations;

namespace KwikNesta.Property.Svc.Domain.Models
{
    public class PropertyFeature : BaseEntity
    {
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int Toilets { get; set; }
        public double AreaSize { get; set; }
        [MaxLength(10)]
        public string AreaUnit { get; set; } = "sqm";
        public bool HasParking { get; set; }
        public bool HasWaterSupply { get; set; }
        public bool HasElectricity { get; set; }

        // Navigation
        public ICollection<RealEstateProperty> Properties { get; set; } = [];
    }
}