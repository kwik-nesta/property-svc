using KwikNesta.Property.Svc.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KwikNesta.Property.Svc.Domain.Models
{
    public class ViewingRequest : BaseEntity
    {
        public Guid PropertyId { get; set; }
        public Property Property { get; set; } = default!;

        [Required]
        public string RequestedById { get; set; } = string.Empty;
        [Required]
        public DateTime RequestedDate { get; set; }
        [Required]
        [EnumDataType(typeof(ViewingStatus))]
        [Column(TypeName = "varchar(20)")] 
        public ViewingStatus Status { get; set; } = ViewingStatus.Pending;
        [MaxLength(1000)]
        public string? Notes { get; set; }
        public DateTime? RespondedAt { get; set; }
    }
}