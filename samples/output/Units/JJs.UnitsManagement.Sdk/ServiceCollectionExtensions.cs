using JJs.Shared.Authentication.Extensions;
using JJs.Shared.Authentication.Handlers;
using JJs.UnitsManagement.Sdk.Sync;
using JJs.UnitsManagement.Sdk.Unit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Serilog;

namespace JJs.UnitsManagement.Sdk;

/// <summary>
/// Extension methods for configuring Units SDK services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Checks if a typed HttpClient service is already registered
    /// </summary>
    private static bool IsHttpClientRegistered<TClient>(this IServiceCollection services) where TClient : class
    {
        return services.Any(sd => sd.ServiceType == typeof(TClient));
    }

    private static IServiceCollection AddUnitsApiClients(this IServiceCollection services, string baseUrl)
    {
        Log.Information("[Units.Sdk] AddUnitsApiClients - Configuring HttpClients with baseUrl: '{BaseUrl}'", baseUrl);

        // Configure HTTP clients for API communication
        // Use idempotent registration to prevent errors when SDK is registered by multiple plugins
        if (!services.IsHttpClientRegistered<UnitApiService>())
        {
            services.AddHttpClient<UnitApiService>(client =>
            {
                Log.Information("[Units.Sdk] Configuring UnitApiService HttpClient - BaseAddress: '{BaseUrl}'", baseUrl);
                client.BaseAddress = new(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddHttpMessageHandler<PluginAuthenticationHandler>();
        }

        if (!services.IsHttpClientRegistered<UnitsSyncApiService>())
        {
            services.AddHttpClient<UnitsSyncApiService>(client =>
            {
                Log.Information("[Units.Sdk] Configuring UnitsSyncApiService HttpClient - BaseAddress: '{BaseUrl}'", baseUrl);
                client.BaseAddress = new(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddHttpMessageHandler<PluginAuthenticationHandler>();
        }

        Log.Information("[Units.Sdk] AddUnitsApiClients completed");
        return services;
    }

    private static IServiceCollection AddUnitsSdkServices(this IServiceCollection services)
    {
        // Register unit validators - use TryAddScoped for idempotent registration
        services.TryAddScoped<CreateUnitValidator>();
        services.TryAddScoped<ReadUnitValidator>();
        services.TryAddScoped<ReadAllUnitsValidator>();
        services.TryAddScoped<UpdateUnitValidator>();
        services.TryAddScoped<DeleteUnitValidator>();

        // Register sync validators
        services.TryAddScoped<StartSyncJobRequestValidator>();

        // Register API service interfaces
        services.TryAddScoped<IUnitsSyncApiService>(provider => provider.GetRequiredService<UnitsSyncApiService>());

        return services;
    }

    /// <summary>
    /// Adds complete Units SDK with HTTP clients and services
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="baseUrl">The base URL for the Units API</param>
    /// <param name="configuration">The application configuration used to retrieve Azure AD settings for authentication.</param>
    /// <returns>The service collection for method chaining</returns>
    /// <remarks>
    /// This method is a convenience method that adds both HTTP clients and SDK services.
    /// </remarks>
    public static IServiceCollection AddUnitsSdk(this IServiceCollection services, string baseUrl, IConfiguration configuration)
    {
        Log.Information("[Units.Sdk] AddUnitsSdk starting with baseUrl: '{BaseUrl}'", baseUrl);

        // Register IHttpContextAccessor - required by PluginAuthenticationHandler
        services.AddHttpContextAccessor();

        // Register the shared authentication handler
        services.AddPluginAuthenticationHandler(configuration);

        var result = services
            .AddUnitsApiClients(baseUrl)
            .AddUnitsSdkServices();

        Log.Information("[Units.Sdk] AddUnitsSdk completed");
        return result;
    }
}
