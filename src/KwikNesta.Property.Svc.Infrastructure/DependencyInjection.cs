using KwikNesta.Property.Svc.Application.Common.Interfaces;
using KwikNesta.Property.Svc.Infrastructure.Persistence;
using KwikNesta.Property.Svc.Infrastructure.Repositories;
using KwikNesta.Property.Svc.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KwikNesta.Property.Svc.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterInfraServices(this IServiceCollection services,
                                                          IConfiguration configuration)
        {
            services.AddScoped<IRepositoryManager, RepositoryManager>()
                .AddScoped<IServiceManager, ServiceManager>()
                .AddScoped<IUploadService, UploadService>()
                .ConfigureDbContext(configuration);

            return services;
        }

        private static IServiceCollection ConfigureDbContext(this IServiceCollection services,
                                                            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}