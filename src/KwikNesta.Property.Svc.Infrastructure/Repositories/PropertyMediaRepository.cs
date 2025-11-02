using KwikNesta.Property.Svc.Application.Common.Interfaces;
using KwikNesta.Property.Svc.Domain.Models;
using KwikNesta.Property.Svc.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KwikNesta.Property.Svc.Infrastructure.Repositories
{
    public class PropertyMediaRepository : BaseRepository<PropertyMedia>, IPropertyMediaRepository
    {
        public PropertyMediaRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task AddRangeAsync(List<PropertyMedia> media,
                                   bool save = true)
        {
            await base.AddRangeAsync(media, save);
        }

        public async Task<PropertyMedia?> GetAsync(Guid id)
        {
            return await GetByIdAsync(id);
        }

        public async Task DeleteAsync(PropertyMedia media, bool save = true)
        {
            await base.Delete(media, save);
        }

        public async Task<List<PropertyMedia>> GetRangeAsync(Guid propertyId, CancellationToken token = default)
        {
            return await base.GetAsQueryable(m => m.PropertyId == propertyId)
                .ToListAsync(token);
        }

        public async Task UpdateAsync(PropertyMedia media,
                                      bool save = true)
        {
            await base.Update(media, save);
        }
    }
}
