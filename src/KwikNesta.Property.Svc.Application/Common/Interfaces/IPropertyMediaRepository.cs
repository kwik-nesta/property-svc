using KwikNesta.Property.Svc.Domain.Models;

namespace KwikNesta.Property.Svc.Application.Common.Interfaces
{
    public interface IPropertyMediaRepository
    {
        Task AddRangeAsync(List<PropertyMedia> media, bool save = true);
        Task DeleteAsync(PropertyMedia media, bool save = true);
        Task<PropertyMedia?> GetAsync(Guid id);
        Task<List<PropertyMedia>> GetRangeAsync(Guid propertyId, CancellationToken token = default);
        Task UpdateAsync(PropertyMedia media, bool save = true);
    }
}