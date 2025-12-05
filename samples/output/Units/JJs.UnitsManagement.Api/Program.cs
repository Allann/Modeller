// Force deploy: 2025-12-01 15:51:34
// Force deployment: 2025-12-01T3
using System.Diagnostics;
using JJs.UnitsManagement.Api;
using JJs.UnitsManagement.Api.Configuration;
using JJs.UnitsManagement.Api.Configuration.Options;
using JJs.UnitsManagement.Api.Extensions;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
    .Enrich.With(new RedactEnrichLogs(["ApiKey", "x-api-key"]))
    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(context.Configuration)
        .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName));

builder.AddServiceDefaults();

builder.Services.Configure<DiagnosticsOptions>(builder.Configuration.GetSection(nameof(DiagnosticsOptions)));
builder.Services.AddSingleton(new ActivitySource("UnitsManagement"));

builder.AddApplicationDependencies();

var app = builder.Build();

try
{
    Log.Information("Starting {AppName}", builder.Environment.ApplicationName);

    app.UsePipeline();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}

// used for integration tests startup
public partial class Program
{
}
