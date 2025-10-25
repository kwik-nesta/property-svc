using System.ComponentModel.DataAnnotations;

namespace KwikNesta.Property.Svc.Domain.Models
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool IsDeprecated { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedOn { get; set; } = DateTime.UtcNow;
    }
}