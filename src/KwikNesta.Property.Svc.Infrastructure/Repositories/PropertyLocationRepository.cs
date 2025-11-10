using KwikNesta.Property.Svc.Application.Common.Interfaces;
using KwikNesta.Property.Svc.Domain.Models;
using KwikNesta.Property.Svc.Infrastructure.Persistence;

namespace KwikNesta.Property.Svc.Infrastructure.Repositories
{
    public class PropertyLocationRepository : BaseRepository<PropertyLocation>, IPropertyLocationRepository
    {
        public PropertyLocationRepository(AppDbContext dbContext)
            : base(dbContext) { }

        public async Task UpdateAsync(PropertyLocation entity) =>
            await base.Update(entity);
    }
}