using System.ComponentModel;

namespace KwikNesta.Property.Svc.Domain.Enums
{
    public enum ListingStatus
    {
        [Description("Pending")]
        Pending,
        [Description("Available")]
        Available,
        [Description("Sold")]
        Sold,
        [Description("Rented")]
        Rented,
        [Description("Withdrawn")]
        Withdrawn,
        [Description("Archived")]
        Archived
    }
}
