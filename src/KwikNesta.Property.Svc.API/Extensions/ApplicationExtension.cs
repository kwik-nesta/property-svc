using DiagnosKit.Core.Logging.Contracts;
using Hangfire;
using KwikNesta.Property.Svc.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KwikNesta.Property.Svc.API.Extensions
{
    public static class ApplicationExtension
    {
        public static WebApplication UseMiddlewares(this WebApplication app,
                                                    IConfiguration configuration)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("GatewayOnly");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.RunMigrations(true);
            app.MapControllers();
            app.MapGet("/", () =>
            {
                return Results.Ok(new
                {
                    Successful = true,
                    Status = 200,
                    Message = $"Kwik Nesta Property service running in {app.Environment.EnvironmentName} mode..."
                });
            });

            return app;
        }

        internal static WebApplication RunMigrations(this WebApplication app, bool alwayRun = false)
        {
            if (app.Environment.IsDevelopment() || alwayRun)
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }

            return app;
        }
    }
}