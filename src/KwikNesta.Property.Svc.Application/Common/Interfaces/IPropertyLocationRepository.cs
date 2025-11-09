using KwikNesta.Property.Svc.Domain.Models;

namespace KwikNesta.Property.Svc.Application.Common.Interfaces
{
    public interface IPropertyLocationRepository
    {
        Task UpdateAsync(PropertyLocation entity);
    }
}