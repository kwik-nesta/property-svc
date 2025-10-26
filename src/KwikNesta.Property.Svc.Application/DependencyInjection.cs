using CrossQueue.Hub.Models;
using CrossQueue.Hub.Shared.Extensions;
using Hangfire;
using Hangfire.Console;
using Hangfire.PostgreSql;
using KwikNesta.Contracts.Http;
using KwikNesta.Contracts.Settings;
using KwikNesta.Mediatrix.Core.Abstractions;
using KwikNesta.Mediatrix.Core.Extensions;
using KwikNesta.Mediatrix.Core.Implementations.Pipelines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System.Text.Json.Serialization;
using System.Text.Json;
using KwikNesta.Property.Svc.Application.Common.Interfaces;

namespace KwikNesta.Property.Svc.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterAppServices(this IServiceCollection services,
                                                             IConfiguration configuration)
        {
            return services
                .ConfigureKwikMediator()
                .AddCrossQueueHubRabbitMqBus(opt =>
                {
                    var settings = configuration.GetSection("RabbitMQ")
                        .Get<QueueSettings>() ?? throw new ArgumentNullException("RabbitMQ");
                    opt.RabbitMQ = new RabbitMQOptions
                    {
                        ConnectionString = settings.ConnectionString,
                        ConsumerRetryCount = 5,
                        ConsumerRetryDelayMs = 500,
                        Durable = true,
                        PublishRetryCount = 5,
                        PublishRetryDelayMs = 500,
                        DefaultExchangeType = settings.ExchangeType,
                        DeadLetterExchange = settings.DeadLetterExchange,
                        DefaultExchange = settings.Exchange
                    };
                })
                .ConfigureHangfire(configuration)
                .ConfigureRefit(configuration);
        }

        private static IServiceCollection ConfigureKwikMediator(this IServiceCollection services)
        {
            services
                .AddKwikMediators(typeof(ApplicationAssemblyMarker).Assembly)
                .AddTransient(typeof(IKwikPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            return services;
        }

        private static IServiceCollection ConfigureHangfire(this IServiceCollection services,
                                                           IConfiguration configuration)
        {
            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(opt =>
                    {
                        opt.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
                    }, new PostgreSqlStorageOptions
                    {
                        SchemaName = "property-svc-hangfire",
                        PrepareSchemaIfNecessary = true
                    })
                    .UseConsole()
                    .UseFilter(new AutomaticRetryAttribute()
                    {
                        Attempts = 5,
                        DelayInSecondsByAttemptFunc = _ => 60
                    });
            }).AddHangfireServer(opt =>
            {
                opt.ServerName = "Kwik Nesta Property Svc";
                opt.Queues = new[] { "recurring", "default" };
                opt.SchedulePollingInterval = TimeSpan.FromMinutes(1);
                opt.WorkerCount = 5;
            });

            return services;
        }

        private static IServiceCollection ConfigureRefit(this IServiceCollection services,
                                                        IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddTransient<ForwardAuthHeaderHandler>();

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };

            var refitSettings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(options)
            };

            var servers = configuration.GetSection("ServiceUrls")
                .Get<ServiceUrls>() ??
                throw new ArgumentNullException("ServiceUrls");

            services.AddRefitClient<ILocationClientService>(refitSettings)
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(servers.SupportService);
                    c.Timeout = TimeSpan.FromSeconds(120);
                }).AddHttpMessageHandler(() => new ServiceRefitWakeUpHandler());

            return services;
        }

    }
}