using System.CommandLine;
using Modeller.Generator.Configuration;

namespace Modeller.Cli.Commands;

/// <summary>
/// modeller init - Initialize a new .modeller configuration
/// </summary>
public static class InitCommand
{
    public static Command Create()
    {
        var templateSourceOption = new Option<string>(
            aliases: ["--template-source", "-t"],
            description: "Source path/URL for templates (e.g., file://C:/templates)")
        {
            IsRequired = true
        };

        var domainOption = new Option<string>(
            aliases: ["--domain", "-d"],
            description: "Path to domain definitions folder",
            getDefaultValue: () => ".");

        var packOption = new Option<string>(
            aliases: ["--pack", "-p"],
            description: "Template pack to use (e.g., csharp/clean-architecture)")
        {
            IsRequired = true
        };

        var forceOption = new Option<bool>(
            aliases: ["--force", "-f"],
            description: "Overwrite existing configuration");

        var command = new Command("init", "Initialize a new .modeller configuration")
        {
            templateSourceOption,
            domainOption,
            packOption,
            forceOption
        };

        command.SetHandler(ExecuteAsync, templateSourceOption, domainOption, packOption, forceOption);

        return command;
    }

    private static async Task ExecuteAsync(string templateSource, string domain, string pack, bool force)
    {
        var projectRoot = Directory.GetCurrentDirectory();
        var modellerFolder = Path.Combine(projectRoot, ".modeller");

        // Check if already initialized
        if (Directory.Exists(modellerFolder) && !force)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Configuration already exists. Use --force to overwrite.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine($"Initializing Modeller in {projectRoot}");
        Console.WriteLine($"  Template source: {templateSource}");
        Console.WriteLine($"  Pack: {pack}");
        Console.WriteLine($"  Domain: {domain}");

        // Create .modeller folder structure
        Directory.CreateDirectory(modellerFolder);
        Directory.CreateDirectory(Path.Combine(modellerFolder, "profiles"));
        Directory.CreateDirectory(Path.Combine(modellerFolder, "templates"));

        // Copy templates from source
        var sourcePath = templateSource.Replace("file://", "");
        var packSourcePath = Path.Combine(sourcePath, pack);

        if (!Directory.Exists(packSourcePath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Pack not found: {packSourcePath}");
            Console.ResetColor();
            return;
        }

        // Copy pack templates
        var packDestPath = Path.Combine(modellerFolder, "templates", pack);
        CopyDirectory(packSourcePath, packDestPath);
        Console.WriteLine($"  Copied pack: {pack}");

        // Copy snippets if they exist
        var snippetsSourcePath = Path.Combine(sourcePath, "_snippets");
        if (Directory.Exists(snippetsSourcePath))
        {
            var snippetsDestPath = Path.Combine(modellerFolder, "templates", "_snippets");
            CopyDirectory(snippetsSourcePath, snippetsDestPath);
            Console.WriteLine("  Copied snippets");
        }

        // Create default config.yaml
        var configContent = $$"""
            # Modeller Configuration
            version: 1

            domain: {{domain}}
            template_source: {{templateSource}}

            variables:
              company: MyCompany
              product: MyProduct
              copyright: "© {{DateTime.Now.Year}} MyCompany"
              root_namespace: MyCompany.MyProduct

            output:
              root: ./src
              project_pattern: "{variables.company}.{variables.product}.{layer}"

            default_profile: default

            files:
              generated_suffix: ".g"
              line_ending: auto
              encoding: utf-8
            """;

        await File.WriteAllTextAsync(Path.Combine(modellerFolder, "config.yaml"), configContent);
        Console.WriteLine("  Created config.yaml");

        // Create default profile
        var profileContent = $$"""
            name: Default
            description: Default generation profile
            pack: {{pack}}

            layers:
              - name: Domain
                template: domain
                output: "{variables.company}.{variables.product}.Domain"

            include:
              entities: all
              enums: all
              commands: all
              queries: all
              projections: all
            """;

        await File.WriteAllTextAsync(
            Path.Combine(modellerFolder, "profiles", "default.yaml"),
            profileContent);
        Console.WriteLine("  Created profiles/default.yaml");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nInitialization complete!");
        Console.WriteLine("Edit .modeller/config.yaml to configure your project.");
        Console.ResetColor();
    }

    private static void CopyDirectory(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);

        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var destFile = Path.Combine(destDir, Path.GetFileName(file));
            File.Copy(file, destFile, true);
        }

        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            var destSubDir = Path.Combine(destDir, Path.GetFileName(dir));
            CopyDirectory(dir, destSubDir);
        }
    }
}

