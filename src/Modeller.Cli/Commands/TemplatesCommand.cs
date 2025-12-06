using System.CommandLine;
using Modeller.Generator.Configuration;
using Modeller.Generator.Templates;

namespace Modeller.Cli.Commands;

/// <summary>
/// modeller templates - Manage and view templates
/// </summary>
public static class TemplatesCommand
{
    public static Command Create()
    {
        var command = new Command("templates", "Manage and view templates");

        command.AddCommand(CreateListCommand());
        command.AddCommand(CreateInfoCommand());

        return command;
    }

    private static Command CreateListCommand()
    {
        var sourceOption = new Option<string?>(
            aliases: ["--source", "-s"],
            description: "Template source path (default: from .modeller/config.yaml or ./templates)");

        var command = new Command("list", "List available template packs")
        {
            sourceOption
        };

        command.SetHandler(ExecuteListAsync, sourceOption);

        return command;
    }

    private static async Task ExecuteListAsync(string? source)
    {
        var templatesPath = ResolveTemplatesPath(source);

        if (!Directory.Exists(templatesPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Templates folder not found: {templatesPath}");
            Console.ResetColor();
            return;
        }

        Console.WriteLine($"Templates in: {templatesPath}\n");

        var discovery = new TemplateDiscovery(templatesPath);
        var packs = discovery.DiscoverPacks();

        if (!packs.Any())
        {
            Console.WriteLine("No template packs found.");
            return;
        }

        foreach (var pack in packs)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"  {pack.Name}");
            Console.ResetColor();
            Console.WriteLine($" ({pack.Manifest?.Description ?? "No description"})");

            if (pack.Templates.Any())
            {
                Console.WriteLine($"    Templates: {string.Join(", ", pack.Templates.Select(t => t.Name))}");
            }
        }

        await Task.CompletedTask;
    }

    private static Command CreateInfoCommand()
    {
        var packArgument = new Argument<string>(
            name: "pack",
            description: "Pack name (e.g., csharp/clean-architecture)");

        var sourceOption = new Option<string?>(
            aliases: ["--source", "-s"],
            description: "Template source path");

        var command = new Command("info", "Show detailed information about a template pack")
        {
            packArgument,
            sourceOption
        };

        command.SetHandler(ExecuteInfoAsync, packArgument, sourceOption);

        return command;
    }

    private static async Task ExecuteInfoAsync(string pack, string? source)
    {
        var templatesPath = ResolveTemplatesPath(source);

        if (!Directory.Exists(templatesPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Templates folder not found: {templatesPath}");
            Console.ResetColor();
            return;
        }

        var discovery = new TemplateDiscovery(templatesPath);
        var packs = discovery.DiscoverPacks();
        var foundPack = packs.FirstOrDefault(p =>
            p.Name.Equals(pack, StringComparison.OrdinalIgnoreCase) ||
            p.Path.EndsWith(pack.Replace('/', Path.DirectorySeparatorChar)));

        if (foundPack == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Pack not found: {pack}");
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Pack: {foundPack.Name}");
        Console.ResetColor();

        if (foundPack.Manifest != null)
        {
            Console.WriteLine($"Description: {foundPack.Manifest.Description}");
            Console.WriteLine($"Version: {foundPack.Manifest.Version}");

            if (foundPack.Manifest.Variables.Any())
            {
                Console.WriteLine("\nDefault Variables:");
                foreach (var kvp in foundPack.Manifest.Variables)
                {
                    Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
                }
            }
        }

        Console.WriteLine($"\nTemplates ({foundPack.Templates.Count}):");
        foreach (var template in foundPack.Templates)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"  {template.Name}");
            Console.ResetColor();

            if (template.Manifest != null)
            {
                Console.WriteLine($" - {template.Manifest.Description}");
                Console.WriteLine($"    Generates: {string.Join(", ", template.Manifest.Generates.Select(f => f.Output))}");
            }
            else
            {
                Console.WriteLine();
            }
        }

        await Task.CompletedTask;
    }

    private static string ResolveTemplatesPath(string? source)
    {
        // If source specified, use it
        if (!string.IsNullOrEmpty(source))
        {
            return source.Replace("file://", "");
        }

        // Try to load from .modeller config
        var projectRoot = Directory.GetCurrentDirectory();
        var loader = new ConfigLoader();

        if (loader.HasConfiguration(projectRoot))
        {
            var config = loader.LoadConfiguration(projectRoot);
            return config.GetTemplatesPath();
        }

        // Fall back to ./templates
        return Path.Combine(projectRoot, "templates");
    }
}

