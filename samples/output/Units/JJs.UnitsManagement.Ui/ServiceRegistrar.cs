// Force deployment: 2025-12-01T3
using JJs.UnitsManagement.Ui.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Serilog;

using Shell.Plugin;

namespace JJs.UnitsManagement.Ui;

/// <summary>
/// Service registrar for the UnitsManagement UI module.
/// Force deployment: 2025-12-01
/// </summary>
public class ServiceRegistrar : IServiceRegistrar
{
    /// <summary>
    /// Registers server-side services for the UnitsManagement UI module
    /// </summary>
    /// <param name="builder">The host application builder</param>
    public void RegisterServerServices(IHostApplicationBuilder builder)
    {
        // Get the directory where this plugin assembly is located
        var assemblyLocation = typeof(ServiceRegistrar).Assembly.Location;
        var pluginDirectory = Path.GetDirectoryName(assemblyLocation) ?? AppContext.BaseDirectory;

        Log.Information("[Units.ServiceRegistrar] Plugin assembly location: {Location}", assemblyLocation);
        Log.Information("[Units.ServiceRegistrar] Plugin directory: {Directory}", pluginDirectory);

        var configPath = Path.Combine(pluginDirectory, "units.json");
        Log.Information("[Units.ServiceRegistrar] Looking for config at: {ConfigPath}", configPath);
        Log.Information("[Units.ServiceRegistrar] Config file exists: {Exists}", File.Exists(configPath));

        if (File.Exists(configPath))
        {
            Log.Information("[Units.ServiceRegistrar] Loading configuration from: {ConfigPath}", configPath);
            builder.Configuration.AddJsonFile(configPath, optional: true, reloadOnChange: true);

            var envConfigPath = Path.Combine(pluginDirectory, $"units.{builder.Environment.EnvironmentName}.json");
            if (File.Exists(envConfigPath))
            {
                Log.Information("[Units.ServiceRegistrar] Loading environment config from: {EnvConfigPath}", envConfigPath);
                builder.Configuration.AddJsonFile(envConfigPath, optional: true, reloadOnChange: true);
            }
        }
        else
        {
            Log.Warning("[Units.ServiceRegistrar] Configuration file not found at: {ConfigPath}", configPath);
        }

        builder.AddApplicationServices();
    }

    /// <summary>
    /// Provides development authentication profiles for Units Management plugin testing.
    /// This method is discovered by convention and called during dev authentication setup.
    /// </summary>
    /// <returns>Collection of dev authentication profiles specific to Units Management plugin</returns>
    /// <remarks>
    /// Units Management plugin has three user types:
    /// 1. Administrator - Full access to all units management features
    /// 2. Manager - Can manage units and view reports
    /// 3. Viewer - Read-only access to units data
    /// </remarks>
    public static IEnumerable<DevAuthProfile> GetDevAuthProfiles()
    {
        // Administrator - Full access to all units management features
        yield return new DevAuthProfile
        {
            ProfileName = "units-admin",
            Name = "Units Administrator",
            Email = "units.admin@jjswaste.com.au",
            Id = "dev-units-admin-001",
            Roles = new[] { "Plugin.Units.Administrator", "User" },
            CustomClaims = new Dictionary<string, string>
            {
                { "department", "Fleet Management" },
                { "access_level", "full" },
                { "plugin", "Units" }
            }
        };

        // ReadWrite - Can manage units and view reports
        yield return new DevAuthProfile
        {
            ProfileName = "units-readwrite",
            Name = "Units Manager",
            Email = "units.manager@jjswaste.com.au",
            Id = "dev-units-manager-001",
            Roles = new[] { "Plugin.Units.ReadWrite", "User" },
            CustomClaims = new Dictionary<string, string>
            {
                { "department", "Operations" },
                { "access_level", "readwrite" },
                { "plugin", "Units" }
            }
        };

        // Read - Read-only access
        yield return new DevAuthProfile
        {
            ProfileName = "units-read",
            Name = "Units Viewer",
            Email = "units.viewer@jjswaste.com.au",
            Id = "dev-units-viewer-001",
            Roles = new[] { "Plugin.Units.Read", "User" },
            CustomClaims = new Dictionary<string, string>
            {
                { "department", "Dispatch" },
                { "access_level", "read" },
                { "plugin", "Units" }
            }
        };
    }
}

/// <summary>
/// Represents a development authentication profile for testing.
/// This is a local copy of the Shell's DevAuthProfile class to avoid circular dependencies.
/// The Shell discovers this via reflection and converts it to its internal format.
/// </summary>
/// <remarks>
/// This class is intentionally duplicated to avoid plugin -> Shell dependencies.
/// Since profiles are passed as JSON, only the structure matters, not the type.
/// </remarks>
public class DevAuthProfile
{
    /// <summary>
    /// Unique identifier for the profile (used in URLs and form submissions).
    /// Should be lowercase with hyphens (e.g., "units-admin").
    /// </summary>
    public required string ProfileName { get; init; }

    /// <summary>
    /// Display name for the profile (shown in UI).
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Email address for the profile.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Unique user ID for the profile.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Array of roles assigned to this profile.
    /// These will be added as ClaimTypes.Role claims.
    /// </summary>
    public required string[] Roles { get; init; }

    /// <summary>
    /// Custom claims to add to the profile.
    /// These will be added as custom claims with the specified key-value pairs.
    /// </summary>
    public required Dictionary<string, string> CustomClaims { get; init; }
}
