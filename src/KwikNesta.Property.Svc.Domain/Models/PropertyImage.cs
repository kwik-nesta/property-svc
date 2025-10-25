using System.ComponentModel.DataAnnotations;

namespace KwikNesta.Property.Svc.Domain.Models
{
    public class PropertyImage : BaseEntity
    {
        public Guid PropertyId { get; set; }
        public RealEstateProperty? Property { get; set; }

        [Required, Url, MaxLength(500)]
        public string Url { get; set; } = string.Empty;
        public string? PublicId { get; set; }
        public bool IsCover { get; set; }
    }
}