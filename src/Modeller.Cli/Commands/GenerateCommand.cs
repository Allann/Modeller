using System.CommandLine;
using Modeller.Generator.Configuration;
using Modeller.Generator.Generation;

namespace Modeller.Cli.Commands;

/// <summary>
/// modeller generate - Generate code from domain definitions
/// </summary>
public static class GenerateCommand
{
    public static Command Create()
    {
        var profileOption = new Option<string?>("--profile")
        {
            Description = "Profile to use (default: from config.yaml)"
        };
        profileOption.Aliases.Add("-p");

        var dryRunOption = new Option<bool>("--dry-run")
        {
            Description = "Preview what would be generated without writing files"
        };
        dryRunOption.Aliases.Add("-n");

        var layerOption = new Option<string?>("--layer")
        {
            Description = "Generate only a specific layer"
        };
        layerOption.Aliases.Add("-l");

        var variablesOption = new Option<string[]>("--var")
        {
            Description = "Override variables (format: key=value)",
            AllowMultipleArgumentsPerToken = true
        };
        variablesOption.Aliases.Add("-v");

        var command = new Command("generate", "Generate code from domain definitions")
        {
            profileOption,
            dryRunOption,
            layerOption,
            variablesOption
        };

        command.SetAction(async (parseResult, ct) => await ExecuteAsync(
            parseResult.GetValue(profileOption),
            parseResult.GetValue(dryRunOption),
            parseResult.GetValue(layerOption),
            parseResult.GetValue(variablesOption) ?? []));

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

            // Load domain
            Console.WriteLine("\nLoading domain definitions...");
            var domainLoader = new DomainLoader();
            var domain = domainLoader.LoadDomain(config.GetDomainPath());
            var stats = domainLoader.GetStats(domain);
            Console.WriteLine($"  Domain: {domain.Name}");
            Console.WriteLine($"  Entities: {stats.Entities}, Enums: {stats.Enums}, Commands: {stats.Commands}, Queries: {stats.Queries}");

            // Create planner and executor
            var templatesPath = config.GetTemplatesPath();
            var planner = new GenerationPlanner(templatesPath);
            var executor = new GenerationExecutor(templatesPath, config.ProjectConfig.Files);

            Console.WriteLine($"\nGenerating {layersToGenerate.Count} layer(s):");

            var totalCreated = 0;
            var totalOverwritten = 0;
            var totalSkipped = 0;
            var totalFailed = 0;

            foreach (var layerConfig in layersToGenerate)
            {
                Console.WriteLine($"\n  [{layerConfig.Name}]");

                // Create plan for this layer
                var plan = planner.CreatePlan(
                    domain,
                    layerConfig,
                    profileConfig,
                    config.ProjectConfig,
                    config.GetOutputRoot(),
                    cliOverrides);

                Console.WriteLine($"    Files to generate: {plan.Files.Count}");

                // Execute the plan
                var results = await executor.ExecuteAsync(plan, dryRun, result =>
                {
                    var symbol = result.Action switch
                    {
                        GenerationAction.Created => "+",
                        GenerationAction.Overwritten => "~",
                        GenerationAction.Skipped => ".",
                        GenerationAction.Failed => "X",
                        GenerationAction.DryRun => "?",
                        _ => " "
                    };

                    var color = result.Action switch
                    {
                        GenerationAction.Created => ConsoleColor.Green,
                        GenerationAction.Overwritten => ConsoleColor.Yellow,
                        GenerationAction.Skipped => ConsoleColor.DarkGray,
                        GenerationAction.Failed => ConsoleColor.Red,
                        _ => ConsoleColor.Gray
                    };

                    Console.ForegroundColor = color;
                    Console.WriteLine($"    [{symbol}] {result.File.RelativePath}");
                    Console.ResetColor();

                    if (result.Action == GenerationAction.Failed && result.Error != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"        Error: {result.Error}");
                        Console.ResetColor();
                    }
                });

                // Tally results
                foreach (var result in results)
                {
                    switch (result.Action)
                    {
                        case GenerationAction.Created: totalCreated++; break;
                        case GenerationAction.Overwritten: totalOverwritten++; break;
                        case GenerationAction.Skipped: totalSkipped++; break;
                        case GenerationAction.Failed: totalFailed++; break;
                    }
                }
            }

            // Summary
            Console.WriteLine();
            if (dryRun)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[DRY RUN COMPLETE - No files were written]");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Generation complete!");
            }
            Console.ResetColor();
            Console.WriteLine($"  Created: {totalCreated}, Overwritten: {totalOverwritten}, Skipped: {totalSkipped}, Failed: {totalFailed}");
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
