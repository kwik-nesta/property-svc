using KwikNesta.Property.Svc.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KwikNesta.Property.Svc.Domain.Models
{
    public class PropertyMedia : BaseEntity
    {
        public Guid PropertyId { get; set; }
        public RealEstateProperty? Property { get; set; }

        [Required, Url, MaxLength(500)]
        public string Url { get; set; } = string.Empty;
        public string? PublicId { get; set; }
        [Required]
        [EnumDataType(typeof(MediaType))]
        [Column(TypeName = "varchar(20)")]
        public MediaType Type { get; set; }
        public bool IsCover { get; set; }
    }
}
