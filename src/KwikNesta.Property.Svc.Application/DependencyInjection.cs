using KwikNesta.Mediatrix.Core.Abstractions;
using KwikNesta.Mediatrix.Core.Extensions;
using KwikNesta.Mediatrix.Core.Implementations.Pipelines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KwikNesta.Property.Svc.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterAppServices(this IServiceCollection services,
                                                             IConfiguration configuration)
        {
            return services
                .ConfigureKwikMediator();
        }

        private static IServiceCollection ConfigureKwikMediator(this IServiceCollection services)
        {
            services
                .AddKwikMediators(typeof(ApplicationAssemblyMarker).Assembly)
                .AddTransient(typeof(IKwikPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            return services;
        }
    }
}