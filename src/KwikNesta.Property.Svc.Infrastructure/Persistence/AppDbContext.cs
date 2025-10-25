using KwikNesta.Property.Svc.Domain.Models;
using Microsoft.EntityFrameworkCore;
using PropertyEntity = KwikNesta.Property.Svc.Domain.Models.Property;
namespace KwikNesta.Property.Svc.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PropertyEntity> Properties => Set<PropertyEntity>();
        public DbSet<PropertyLocation> PropertyLocations => Set<PropertyLocation>();
        public DbSet<PropertyFeature> PropertyFeatures => Set<PropertyFeature>();
        public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
        public DbSet<ViewingRequest> ViewingRequests => Set<ViewingRequest>();
    }
}