using KwikNesta.Property.Svc.Domain.Enums;

namespace KwikNesta.Property.Svc.Application.Validations
{
    public static class PropertyStatusGuard
    {
        private static readonly Dictionary<ListingStatus, ListingStatus[]> _allowedTransitions = new()
        {
            [ListingStatus.Draft] = [ListingStatus.Pending],
            [ListingStatus.Pending] = [ListingStatus.Available, ListingStatus.Withdrawn],
            [ListingStatus.Available] = [ListingStatus.Sold, ListingStatus.Rented, ListingStatus.Withdrawn],
            [ListingStatus.Sold] = [ListingStatus.Archived],
            [ListingStatus.Rented] = [ListingStatus.Archived, ListingStatus.Rented],
            [ListingStatus.Withdrawn] = [ListingStatus.Archived],
        };

        public static bool CanTransition(ListingStatus from, ListingStatus to)
        {
            return _allowedTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);
        }
    }
}