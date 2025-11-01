using System.ComponentModel;

namespace KwikNesta.Property.Svc.Domain.Enums
{
    public enum ViewingStatus
    {
        [Description("Pending")]
        Pending,
        [Description("Approved")]
        Approved,
        [Description("Rejected")]
        Rejected,
        [Description("Completed")]
        Completed
    }
}
