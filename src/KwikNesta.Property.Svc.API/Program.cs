using DiagnosKit.Core.Logging;
using DiagnosKit.Core.Logging.Contracts;
using DiagnosKit.Core.Middlewares;
using KwikNesta.Property.Svc.API.Extensions;
using KwikNesta.Property.Svc.Application;
using KwikNesta.Property.Svc.Infrastructure;

SerilogBootstrapper.UseBootstrapLogger();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .RegisterApiServices(builder.Configuration)
    .RegisterInfraServices(builder.Configuration)
    .RegisterAppServices(builder.Configuration);

if (!builder.Environment.IsDevelopment())
{
    builder.Host.ConfigureESSink(builder.Configuration);
}

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILoggerManager>();
app.UseDiagnosKitExceptionHandler(logger);
app.UseMiddlewares(builder.Configuration);

app.Run();
