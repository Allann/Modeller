using System.CommandLine;
using Modeller.Generator.Configuration;
using Modeller.Generator.Generation;
using Modeller.Generator.Templates;

namespace Modeller.Cli.Commands;

/// <summary>
/// modeller validate - Validate domain definitions and configuration
/// </summary>
public static class ValidateCommand
{
    public static Command Create()
    {
        var command = new Command("validate", "Validate domain definitions and configuration");

        command.SetHandler(ExecuteAsync);

        return command;
    }

    private static Task ExecuteAsync()
    {
        var projectRoot = Directory.GetCurrentDirectory();
        var loader = new ConfigLoader();

        Console.WriteLine("Validating project configuration...\n");

        var hasErrors = false;
        var hasWarnings = false;

        // Check for .modeller folder
        if (!loader.HasConfiguration(projectRoot))
        {
            WriteError("No .modeller configuration found. Run 'modeller init' first.");
            return Task.CompletedTask;
        }

        WriteSuccess("✓ .modeller folder found");

        try
        {
            // Load and validate configuration
            var config = loader.LoadConfiguration(projectRoot);
            WriteSuccess($"✓ config.yaml loaded (version {config.ProjectConfig.Version})");

            // Validate template source
            var templatesPath = config.GetTemplatesPath();
            if (Directory.Exists(templatesPath))
            {
                WriteSuccess($"✓ Templates found at: {templatesPath}");

                // Discover packs
                var discovery = new TemplateDiscovery(templatesPath);
                var packs = discovery.DiscoverPacks().ToList();
                WriteSuccess($"  Found {packs.Count} template pack(s)");

                foreach (var pack in packs)
                {
                    Console.WriteLine($"    - {pack.Name}: {pack.Templates.Count} template(s)");
                }
            }
            else
            {
                WriteWarning($"⚠ Templates folder not found: {templatesPath}");
                hasWarnings = true;
            }

            // Validate domain path
            var domainPath = config.GetDomainPath();
            if (Directory.Exists(domainPath))
            {
                WriteSuccess($"✓ Domain folder found: {domainPath}");

                // Try to load domain
                var domainLoader = new DomainLoader();
                try
                {
                    var domain = domainLoader.LoadDomain(domainPath);
                    var stats = domainLoader.GetStats(domain);
                    WriteSuccess($"  Domain: {domain.Name}");
                    WriteSuccess($"  Entities: {stats.Entities}, Enums: {stats.Enums}, Commands: {stats.Commands}, Queries: {stats.Queries}");
                }
                catch (Exception ex)
                {
                    WriteError($"✗ Failed to parse domain: {ex.Message}");
                    hasErrors = true;
                }
            }
            else
            {
                WriteError($"✗ Domain folder not found: {domainPath}");
                hasErrors = true;
            }

            // Validate profiles
            var profiles = loader.ListProfiles(config.ModellerFolder).ToList();
            if (profiles.Count > 0)
            {
                WriteSuccess($"✓ Found {profiles.Count} profile(s)");
                foreach (var profileName in profiles)
                {
                    try
                    {
                        var profile = loader.LoadProfile(config.ModellerFolder, profileName);
                        Console.WriteLine($"    - {profileName}: {profile.Layers.Count} layer(s), pack: {profile.Pack}");
                    }
                    catch (Exception ex)
                    {
                        WriteError($"  ✗ {profileName}: {ex.Message}");
                        hasErrors = true;
                    }
                }
            }
            else
            {
                WriteWarning("⚠ No profiles found");
                hasWarnings = true;
            }

            // Validate output path
            var outputRoot = config.GetOutputRoot();
            Console.WriteLine($"\n  Output root: {outputRoot}");
        }
        catch (Exception ex)
        {
            WriteError($"✗ Configuration error: {ex.Message}");
            hasErrors = true;
        }

        // Summary
        Console.WriteLine();
        if (hasErrors)
        {
            WriteError("Validation failed with errors.");
        }
        else if (hasWarnings)
        {
            WriteWarning("Validation passed with warnings.");
        }
        else
        {
            WriteSuccess("Validation passed!");
        }

        return Task.CompletedTask;
    }

    private static void WriteSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private static void WriteWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private static void WriteError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}

