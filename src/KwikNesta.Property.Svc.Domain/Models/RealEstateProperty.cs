using KwikNesta.Property.Svc.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KwikNesta.Property.Svc.Domain.Models
{
    public class RealEstateProperty : BaseEntity
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string? Description { get; set; }
        public decimal Price { get; set; }
        [MaxLength(10)]
        public string Currency { get; set; } = "NGN";
        [Required]
        [EnumDataType(typeof(PropertyType))]
        [Column(TypeName = "varchar(20)")]
        public PropertyType Type { get; set; }
        [Required]
        [EnumDataType(typeof(ListingStatus))]
        [Column(TypeName = "varchar(20)")]
        public ListingStatus Status { get; set; } = ListingStatus.Draft;
        public string? StatusReasons { get; set; }

        public bool IsLocked { get; set; }
        public bool IsCoordinatesSent { get; set; }

        public bool IsOwnerShipVerified { get; set; }
        public string? VerificationReasons { get; set; }


        // Relationships
        public string OwnerId { get; set; } = string.Empty;

        public PropertyLocation Location { get; set; } = default!;
        public PropertyFeature Feature { get; set; } = default!;
        public ICollection<PropertyMedia> Media { get; set; } = [];
        public ICollection<ViewingRequest> ViewingRequests { get; set; } = [];
    }
}