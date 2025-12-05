using System.Diagnostics;

namespace Units.AppHost;

internal static class ResourceBuilderExtensions
{
    internal static IResourceBuilder<T> WithSwagger<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints =>
        builder.WithOpenApiDocs("Swagger", "Swagger API Documentation", "swagger");

    internal static IResourceBuilder<T> WithReDoc<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints =>
        builder.WithOpenApiDocs("ReDoc", "ReDoc API Documentation", "api-docs/index.html");

    internal static IResourceBuilder<T> WithScalar<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints =>
        builder.WithOpenApiDocs("Scalar", "Scalar API Documentation", "scalar/v1");

    private static IResourceBuilder<T> WithOpenApiDocs<T>(this IResourceBuilder<T> builder, string name,
        string displayName, string openApiUiPath)
        where T : IResourceWithEndpoints =>
        builder.WithCommand(name, displayName, _ =>
        {
            try
            {
                // Try to get the HTTPS endpoint with different naming patterns
                var endpoint = TryGetHttpsEndpoint(builder);
                if (endpoint == null)
                {
                    return Task.FromResult(new ExecuteCommandResult
                    {
                        Success = false,
                        ErrorMessage = "No HTTPS endpoint found for this resource"
                    });
                }

                var url = $"{endpoint.Url}/{openApiUiPath}";
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                return Task.FromResult(new ExecuteCommandResult { Success = true });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ExecuteCommandResult { Success = false, ErrorMessage = ex.ToString() });
            }
        }, new()
        {
            Description = $"Open {displayName} in a new browser tab",
            IconName = "Document",
            IconVariant = IconVariant.Filled
        });

    private static EndpointReference? TryGetHttpsEndpoint<T>(IResourceBuilder<T> builder)
        where T : IResourceWithEndpoints
    {
        // Try different HTTPS endpoint naming patterns used across the solution
        var httpsEndpointNames = new[]
        {
            "https",                    // Default pattern
            "customer-https",           // Customer Management pattern
            "organisation-https",       // Organisation Management pattern
            "calendar-https",           // Calendar Management pattern
            "unit-https",              // Units Management pattern
            "videoportal-https",       // Video Portal pattern
            "sla-https"                // SLA Management pattern
        };

        foreach (var endpointName in httpsEndpointNames)
        {
            try
            {
                var endpoint = builder.GetEndpoint(endpointName);
                if (endpoint != null)
                {
                    return endpoint;
                }
            }
            catch
            {
                // Continue trying other endpoint names
                continue;
            }
        }

        return null;
    }
}
