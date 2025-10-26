using KwikNesta.Property.Svc.Domain.Models;
using Microsoft.EntityFrameworkCore;
namespace KwikNesta.Property.Svc.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<RealEstateProperty> Properties => Set<RealEstateProperty>();
        public DbSet<PropertyLocation> PropertyLocations => Set<PropertyLocation>();
        public DbSet<PropertyFeature> PropertyFeatures => Set<PropertyFeature>();
        public DbSet<PropertyMedia> PropertyMedia => Set<PropertyMedia>();
        public DbSet<ViewingRequest> ViewingRequests => Set<ViewingRequest>();

        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("property-svc");

            base.OnModelCreating(builder);
        }
    }
}