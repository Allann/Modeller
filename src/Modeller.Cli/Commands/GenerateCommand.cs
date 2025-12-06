using System.CommandLine;
using Modeller.Generator.Configuration;

namespace Modeller.Cli.Commands;

/// <summary>
/// modeller generate - Generate code from domain definitions
/// </summary>
public static class GenerateCommand
{
    public static Command Create()
    {
        var profileOption = new Option<string?>(
            aliases: ["--profile", "-p"],
            description: "Profile to use (default: from config.yaml)");

        var dryRunOption = new Option<bool>(
            aliases: ["--dry-run", "-n"],
            description: "Preview what would be generated without writing files");

        var layerOption = new Option<string?>(
            aliases: ["--layer", "-l"],
            description: "Generate only a specific layer");

        var variablesOption = new Option<string[]>(
            aliases: ["--var", "-v"],
            description: "Override variables (format: key=value)")
        {
            AllowMultipleArgumentsPerToken = true
        };

        var command = new Command("generate", "Generate code from domain definitions")
        {
            profileOption,
            dryRunOption,
            layerOption,
            variablesOption
        };

        command.SetHandler(ExecuteAsync, profileOption, dryRunOption, layerOption, variablesOption);

        return command;
    }

    private static async Task ExecuteAsync(
        string? profile,
        bool dryRun,
        string? layer,
        string[] variables)
    {
        var projectRoot = Directory.GetCurrentDirectory();
        var loader = new ConfigLoader();

        // Check for configuration
        if (!loader.HasConfiguration(projectRoot))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("No .modeller configuration found. Run 'modeller init' first.");
            Console.ResetColor();
            return;
        }

        try
        {
            // Load configuration
            var config = loader.LoadConfiguration(projectRoot);
            var profileName = profile ?? config.ProjectConfig.DefaultProfile;

            // Load the specified profile
            var profileConfig = profileName != config.ProjectConfig.DefaultProfile
                ? loader.LoadProfile(config.ModellerFolder, profileName)
                : config.DefaultProfile;

            Console.WriteLine($"Generating with profile: {profileConfig.Name}");
            Console.WriteLine($"  Pack: {profileConfig.Pack}");
            Console.WriteLine($"  Domain: {config.GetDomainPath()}");
            Console.WriteLine($"  Output: {config.GetOutputRoot()}");

            if (dryRun)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n[DRY RUN - No files will be written]");
                Console.ResetColor();
            }

            // Parse CLI variable overrides
            var cliOverrides = ParseVariables(variables);
            if (cliOverrides.Count > 0)
            {
                Console.WriteLine("\nVariable overrides:");
                foreach (var kvp in cliOverrides)
                {
                    Console.WriteLine($"  {kvp.Key} = {kvp.Value}");
                }
            }

            // Merge variables
            var merger = new VariableMerger();

            // Filter layers if specified
            var layersToGenerate = profileConfig.Layers;
            if (!string.IsNullOrEmpty(layer))
            {
                layersToGenerate = layersToGenerate
                    .Where(l => l.Name.Equals(layer, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (layersToGenerate.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Layer not found: {layer}");
                    Console.ResetColor();
                    return;
                }
            }

            Console.WriteLine($"\nLayers to generate ({layersToGenerate.Count}):");
            foreach (var layerConfig in layersToGenerate)
            {
                var layerVars = merger.MergeVariables(
                    config.ProjectConfig,
                    profileConfig,
                    layerConfig.Name,
                    cliOverrides);

                var outputPath = merger.ResolvePattern(layerConfig.Output, layerVars);

                Console.WriteLine($"  {layerConfig.Name}:");
                Console.WriteLine($"    Template: {layerConfig.Template}");
                Console.WriteLine($"    Output: {outputPath}");
            }

            if (!dryRun)
            {
                // TODO: Phase 4 - Actual generation
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n[Generation not yet implemented - Phase 4]");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nGeneration plan complete.");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }
    }

    private static Dictionary<string, object> ParseVariables(string[] variables)
    {
        var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        foreach (var variable in variables)
        {
            var parts = variable.Split('=', 2);
            if (parts.Length == 2)
            {
                result[parts[0].Trim()] = parts[1].Trim();
            }
        }

        return result;
    }
}

