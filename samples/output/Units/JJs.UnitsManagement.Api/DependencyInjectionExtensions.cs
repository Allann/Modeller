using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using JJs.Shared.Authentication.Extensions;
using JJs.UnitsManagement.Api.Extensions;
using JJs.UnitsManagement.Api.Services;
using JJs.UnitsManagement.Api.Services.Background;
using JJs.UnitsManagement.Infrastructure;
using JJs.UnitsManagement.Infrastructure.Interceptors;
using JJs.UnitsManagement.Sdk.Sync;
using JJs.UnitsManagement.Sdk.Unit;
using MartinCostello.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Serilog;

namespace JJs.UnitsManagement.Api;

/// <summary>
/// Provides extension methods for configuring dependency injection in an application.
/// </summary>
/// <remarks>This class contains methods that extend the functionality of <see cref="IHostApplicationBuilder"/> 
/// to simplify the registration of application-specific services and dependencies.</remarks>
public static class DependencyInjectionExtensions
{
    /// <summary>
    /// Adds application-specific dependencies to the specified <see cref="IHostApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> to which the dependencies will be added.</param>
    /// <returns>The same <see cref="IHostApplicationBuilder"/> instance, allowing for method chaining.</returns>
    public static IHostApplicationBuilder AddApplicationDependencies(this IHostApplicationBuilder builder)
    {
        _ = builder.AddOpenApi()
            .IncludeProblemDetails()
            .AddDatabase()
            .AddSecurity()
            .Services
                .AddRequestTimeouts()
                .AddOutputCache()
                .AddValidatorsFromAssemblyContaining<CreateUnitRequest>() // SDK validators
                .AddScoped<AuditInterceptor>()
                .AddHttpContextAccessor()
                // Register sync services
                .AddScoped<IUnitsSourceOfTruthService, UnitsSourceOfTruthService>()
                .AddScoped<IUnitsSyncService, UnitsSyncService>()
                .AddScoped<SyncProgressReporter>()
                // Register sync job services
                .AddSingleton<SyncJobService>()
                .AddScoped<ISyncJobService>(provider => provider.GetRequiredService<SyncJobService>())
                // Register background services
                .AddHostedService<Services.Background.SyncJobBackgroundService>()
                .Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
                {
                    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.SerializerOptions.PropertyNameCaseInsensitive = true;
                })
                .AddCors(options =>
                {
                    options.AddPolicy("Development", policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });

                    options.AddPolicy("default", policy =>
                    {
                        policy.WithOrigins("https://localhost:7001", "https://localhost:7002") // Add your client app URLs
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
                });

        return builder;
    }

    public static WebApplication UsePipeline(this WebApplication app)
    {
        const string openApiDocument = "/openapi/v1.json";

        app.MapDefaultEndpoints();
        app.UseHttpsRedirection()
            .UseAuthentication()
            .UseCors(app.Environment.IsDevelopment() ? "Development" : "default")
            .UseAuthorization()
            .UseExceptionHandler()
            .UseStatusCodePages()
            .UseRequestTimeouts()
            .UseOutputCache();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(openApiDocument, "Units Management API");
                options.OAuthClientId(app.Configuration["AzureAd:ClientId"]);
                options.OAuthUsePkce();
            });
            app.UseReDoc(options => options.SpecUrl(openApiDocument));
            app.MapScalarApiReference(options =>
                options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient));
        }

        app.UseSerilogRequestLogging();

        app.MapApiEndpoints();

        return app;
    }

    /// <summary>
    /// Adds authentication and authorisation services using the shared
    /// dual-mode plugin API authentication configuration.
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> to which the security services will be added.</param>
    /// <returns>The same <see cref="IHostApplicationBuilder"/> instance, allowing for method chaining.</returns>
    public static IHostApplicationBuilder AddSecurity(this IHostApplicationBuilder builder)
    {
        builder.AddPluginApiAuthentication("Units");

        // Add authorisation
        builder.Services.AddAuthorization();

        return builder;
    }

    /// <summary>
    /// Adds database services
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> to which the database services will be added.</param>
    /// <returns>The same <see cref="IHostApplicationBuilder"/> instance, allowing for method chaining.</returns>
    public static IHostApplicationBuilder AddDatabase(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContextPool<UnitsManagementDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Realm-Unit"), sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(UnitsManagementDbContext).Assembly.FullName);
            });
            options.AddInterceptors(builder.Services.BuildServiceProvider().GetRequiredService<AuditInterceptor>());
        });
        builder.EnrichSqlServerDbContext<UnitsManagementDbContext>();

        // Add database migrator - runs migrations on startup in non-Development environments
        builder.Services.AddDbMigrator<UnitsManagementDbContext>();

        return builder;
    }

    private static IHostApplicationBuilder AddOpenApi(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddOpenApi(options =>
            {
                options.UseContact();
                options.AddSchemaTransformer<EnumSchemaTransformer>();
            })
            .AddOpenApiExtensions(options =>
            {
                options.AddServerUrls = true;
                options.AddExamples = true;
                options.AddXmlComments<CreateUnitRequest>();
                options.SerializationContexts.Add(new AppJsonSerializerContext());
            })
            .AddHttpContextAccessor();

        return builder;
    }

    /// <summary>
    /// Includes problem details in the application
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder"/> to which problem details will be added.</param>
    /// <returns>The same <see cref="IHostApplicationBuilder"/> instance, allowing for method chaining.</returns>
    public static IHostApplicationBuilder IncludeProblemDetails(this IHostApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

                var activity = Activity.Current;
                context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
            };
        });

        return builder;
    }
}

internal static class OpenApiTransformersExtensions
{
    public static OpenApiOptions UseContact(this OpenApiOptions options)
    {
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Info.Contact = new()
            {
                Name = "JJ's Waste & Recycling",
                Email = "servicedesk@jjswaste.com.au"
            };
            return Task.CompletedTask;
        });
        return options;
    }
}

public class EnumSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        var type = context.GetType();
        if (type.IsEnum)
        {
            schema.Type = "string";
            schema.Format = null;
            schema.Enum.Clear();

            foreach (var name in Enum.GetNames(type))
            {
                schema.Enum.Add(new OpenApiString(name));
            }
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Transformer to add Bearer token security scheme to OpenAPI
/// </summary>
public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    /// <summary>
    /// Transforms the OpenAPI document to include Bearer token security
    /// </summary>
    /// <param name="document">The OpenAPI document</param>
    /// <param name="context">The transformation context</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing the transformation</returns>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var requirements = new Dictionary<string, OpenApiSecurityScheme>
        {
            ["Bearer"] = new()
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                In = ParameterLocation.Header,
                BearerFormat = "Json Web Token"
            }
        };
        document.Components ??= new();
        document.Components.SecuritySchemes = requirements;

        return Task.CompletedTask;
    }
}

/// <summary>
/// JSON serialization context for the Units Management API
/// </summary>
[JsonSerializable(typeof(CreateUnitRequest))]
[JsonSerializable(typeof(UpdateUnitRequest))]
[JsonSerializable(typeof(UnitResponse))]
[JsonSerializable(typeof(UnitListResponse))]
[JsonSerializable(typeof(StartSyncJobRequest))]
[JsonSerializable(typeof(StartSyncJobResponse))]
[JsonSerializable(typeof(SyncProgress))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
