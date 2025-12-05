using JJs.UnitsManagement.Api.Features.Units;
using JJs.UnitsManagement.Api.Features.Sync;

namespace JJs.UnitsManagement.Api.Extensions;

/// <summary>
/// Extension methods for mapping API endpoints
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Maps all API endpoints to the web application
    /// </summary>
    /// <param name="app">The web application instance</param>
    /// <returns>The web application instance for chaining</returns>
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        // Map API endpoints for units management
        app.MapUnitsEndpoints()
           .MapSyncEndpoints();

        return app;
    }
}
