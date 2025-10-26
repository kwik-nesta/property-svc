using KwikNesta.Property.Svc.Application.Common.Interfaces;
using KwikNesta.Property.Svc.Domain.Models;
using KwikNesta.Property.Svc.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KwikNesta.Property.Svc.Infrastructure.Repositories
{
    public class PropertyRepository : BaseRepository<RealEstateProperty>, IPropertyRepository
    {
        public PropertyRepository(AppDbContext dbContext) : base(dbContext) { }

        public async Task CreateAsync(RealEstateProperty property,
                                   bool save = true)
        {
            await base.AddAsync(property, save);
        }

        public async Task<RealEstateProperty?> GetAsync(Guid id, 
                                                        bool eagerLoad = false)
        {
            return eagerLoad ? 
                await GetAsQueryable(p  => p.Id == id)
                    .Include(p => p.Location)
                    .Include(p => p.Feature)
                    .Include(p => p.Media)
                    .Include(p => p.ViewingRequests)
                    .FirstOrDefaultAsync() :
                await GetByIdAsync(id);
        }

        public async Task UpdateAsync(RealEstateProperty property,
                                      bool save = true)
        {
            await base.Update(property, true);
        }
    }
}
