using KwikNesta.Property.Svc.Application.Common.Interfaces;
using KwikNesta.Property.Svc.Domain.Models;
using KwikNesta.Property.Svc.Infrastructure.Persistence;

namespace KwikNesta.Property.Svc.Infrastructure.Repositories
{
    public class PropertyRepository : BaseRepository<RealEstateProperty>, IPropertyRepository
    {
        public PropertyRepository(AppDbContext dbContext) : base(dbContext) { }
    }
}
