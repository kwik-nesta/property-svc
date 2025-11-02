using DiagnosKit.Core.Configurations;
using Cloudtenary.Extensions;
using DiagnosKit.Core.Extensions;
using KwikNesta.Contracts.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using Cloudtenary.Settings;

namespace KwikNesta.Property.Svc.API.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterApiServices(this IServiceCollection services,
                                                             IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer()
                .ConfigureSwagger()
                .AddCloudtenary(opt =>
                {
                    var settings = configuration.GetSection("CloudtenarySettings")
                        .Get<CloudtenarySettings>() ?? throw new ArgumentNullException("CloudtenarySettings");

                    opt.Secret = settings.Secret;
                    opt.CloudName = settings.CloudName;
                    opt.Key = settings.Key;
                })
                .ConfigureApiVersion()
                .ConfigureJwt(configuration)
                .ConfigureCors(configuration)
                
                .AddLoggerManager();
            return services;
        }

        public static void ConfigureESSink(this IHostBuilder host,
                                           IConfiguration configuration)
        {
            host.ConfigureSerilogESSink(opt =>
            {
                var settings = configuration.GetSection("ElasticSearch")
                    .Get<ElasticSettings>() ?? throw new ArgumentNullException("ElasticSearch");

                opt.Url = settings.Url;
                opt.Username = settings.UserName;
                opt.Password = settings.Password;
                opt.IndexPrefix = settings.IndexPrefix;
                opt.IndexFormat = settings.IndexFormat;
            });
        }

        private static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            return services.AddSwaggerGen(c =>
            {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Kwik Nesta Property Svc",
                    Version = "v1",
                    Description = "Kwik Nesta Property Service API v1.0",
                    Contact = new OpenApiContact
                    {
                        Name = "Kwik Nesta Inc.",
                        Email = "info@kwik-nesta.com",
                        Url = new Uri("https://kwik-nesta.com")
                    }
                });
                c.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "Kwik Nesta Property Svc",
                    Version = "v2",
                    Description = "Kwik Nesta Property Service API v2.0",
                    Contact = new OpenApiContact
                    {
                        Name = "Kwik Nesta Inc.",
                        Email = "info@kwik-nesta.com",
                        Url = new Uri("https://kwik-nesta.com")
                    }
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Kwik Nesta Property API"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }});
            });
        }

        private static IServiceCollection ConfigureApiVersion(this IServiceCollection services)
        {
            return services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("api-version"),
                    new HeaderApiVersionReader("X-Version"),
                    new UrlSegmentApiVersionReader());
            });
        }

        private static IServiceCollection ConfigureJwt(this IServiceCollection services,
                                                       IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt")
                .Get<JwtSettings>() ?? throw new ArgumentNullException("JWT Config can not be null");

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,

                        ValidateLifetime = true,

                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,

                        RoleClaimType = jwtSettings.RoleClaim,

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.IssuerSigningKey))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            // stop the default behavior (and the WWW-Authenticate header)
                            context.HandleResponse();
                            var statusCode = StatusCodes.Status401Unauthorized;
                            var message = "Unauthorized. Please login";

                            // Check if the failure is due to token expiration
                            if (context.AuthenticateFailure is SecurityTokenExpiredException)
                            {
                                statusCode = StatusCodes.Status403Forbidden;
                                message = "Forbidden. Token has expired!";
                            }

                            context.Response.StatusCode = statusCode;
                            context.Response.ContentType = "application/json";

                            await context.Response.WriteAsJsonAsync(new
                            {
                                Successful = false,
                                Status = statusCode,
                                Message = message
                            });
                        }
                    };
                });

            services.AddAuthorization();
            return services;
        }

        private static IServiceCollection ConfigureCors(this IServiceCollection services,
                                                        IConfiguration configuration)
        {
            var gatewayUrl = configuration.GetSection("ServiceUrls")
                .Get<ServiceUrls>()?.GatewayService ??
                throw new ArgumentNullException("ServiceUrls");

            services.AddCors(options =>
            {
                options.AddPolicy("GatewayOnly", policy =>
                {
                    policy.WithOrigins(gatewayUrl)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            return services;
        }
    }
}
