using System.ComponentModel;

namespace KwikNesta.Property.Svc.Domain.Enums
{
    public enum ListingStatus
    {
        [Description("Draft")]
        Draft,
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
        Archived,
        [Description("Verification Failed")]
        VerificationFailed
    }
}
