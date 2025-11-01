using KwikNesta.Property.Svc.Domain.Models;

namespace KwikNesta.Property.Svc.Application.Common.Interfaces
{
    public interface IPropertyRepository
    {
        Task CreateAsync(RealEstateProperty property, bool save = true);
        Task<RealEstateProperty?> GetAsync(Guid id, bool eagerLoad = false);
        Task UpdateAsync(RealEstateProperty property, bool save = true);
    }
}
