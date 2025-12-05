using JJs.UnitsManagement.Sdk;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Serilog;

using Shell.Plugin;

namespace JJs.UnitsManagement.Ui.Extensions;

/// <summary>
/// Extension methods for configuring UnitsManagement UI services
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds application services for the UnitsManagement UI module
    /// </summary>
    /// <param name="builder">The host application builder</param>
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        Log.Debug("[Units.Ui] AddApplicationServices starting - resolving API base URL");

        // Log configuration values for debugging
        LogConfigurationValues(builder.Configuration);

        var apiBaseUrl = PluginApiUrlResolver.Resolve(builder.Configuration, "Units", "units-api");

        Log.Debug("[Units.Ui] PluginApiUrlResolver.Resolve returned: '{ApiBaseUrl}'", apiBaseUrl);
        Log.Debug("[Units.Ui] Calling AddUnitsSdk with baseUrl: '{BaseUrl}'", apiBaseUrl);

        builder.Services.AddUnitsSdk(apiBaseUrl, builder.Configuration);

        Log.Debug("[Units.Ui] AddUnitsSdk completed for Units plugin");
    }

    private static void LogConfigurationValues(IConfiguration configuration)
    {
        // Log Priority 1: Plugin-specific configuration
        var pluginConfigKey = "ApiSettings:Units:BaseUrl";
        var pluginBaseUrl = configuration.GetValue<string>(pluginConfigKey);
        Log.Debug("[Units.Ui] Config Priority 1 - '{Key}' = '{Value}'",
            pluginConfigKey, pluginBaseUrl ?? "(null/empty)");

        // Log Priority 2: Shell ApiGateway configuration
        var gatewayBaseUrl = configuration.GetValue<string>("ApiGateway:BaseUrl");
        var apiEndpoint = configuration.GetValue<string>("ApiGateway:ApiEndpoints:units-api");
        Log.Debug("[Units.Ui] Config Priority 2 - 'ApiGateway:BaseUrl' = '{GatewayUrl}', 'ApiGateway:ApiEndpoints:units-api' = '{Endpoint}'",
            gatewayBaseUrl ?? "(null/empty)", apiEndpoint ?? "(null/empty)");

        // Log all ApiSettings keys to help identify configuration issues
        var apiSettingsSection = configuration.GetSection("ApiSettings");
        if (!apiSettingsSection.Exists())
        {
            Log.Warning("[Units.Ui] ApiSettings section does NOT exist in configuration!");
        }

        // Log ApiGateway section if it exists
        var apiGatewaySection = configuration.GetSection("ApiGateway");
        if (apiGatewaySection.Exists())
        {
            Log.Debug("[Units.Ui] ApiGateway section exists. Children:");
            foreach (var child in apiGatewaySection.GetChildren())
            {
                Log.Debug("[Units.Ui]   - {Key}: {Value}",
                    child.Key,
                    child.Value ?? $"(section with {child.GetChildren().Count()} children)");
            }
        }
    }
}
