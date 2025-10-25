using KwikNesta.Property.Svc.Domain.Models;
using Microsoft.EntityFrameworkCore;
namespace KwikNesta.Property.Svc.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<RealEstateProperty> Properties => Set<RealEstateProperty>();
        public DbSet<PropertyLocation> PropertyLocations => Set<PropertyLocation>();
        public DbSet<PropertyFeature> PropertyFeatures => Set<PropertyFeature>();
        public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
        public DbSet<ViewingRequest> ViewingRequests => Set<ViewingRequest>();
    }
}