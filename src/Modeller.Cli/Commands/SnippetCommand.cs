using System.CommandLine;
using Modeller.Generator.Configuration;
using Modeller.Generator.Templates;

namespace Modeller.Cli.Commands;

/// <summary>
/// modeller snippet - List and preview available snippets
/// </summary>
public static class SnippetCommand
{
    public static Command Create()
    {
        var listCommand = CreateListCommand();
        var showCommand = CreateShowCommand();

        var command = new Command("snippet", "Manage template snippets")
        {
            listCommand,
            showCommand
        };

        return command;
    }

    private static Command CreateListCommand()
    {
        var sourceOption = new Option<string?>(
            aliases: ["--source", "-s"],
            description: "Template source path (defaults to project templates)");

        var command = new Command("list", "List available snippets")
        {
            sourceOption
        };

        command.SetHandler(ExecuteListAsync, sourceOption);

        return command;
    }

    private static Command CreateShowCommand()
    {
        var nameArgument = new Argument<string>(
            name: "name",
            description: "Snippet name to show");

        var sourceOption = new Option<string?>(
            aliases: ["--source", "-s"],
            description: "Template source path (defaults to project templates)");

        var command = new Command("show", "Show snippet content")
        {
            nameArgument,
            sourceOption
        };

        command.SetHandler(ExecuteShowAsync, nameArgument, sourceOption);

        return command;
    }

    private static Task ExecuteListAsync(string? source)
    {
        var templatesPath = ResolveTemplatesPath(source);
        if (templatesPath == null) return Task.CompletedTask;

        var snippetLoader = new SnippetLoader(templatesPath);
        var snippets = snippetLoader.DiscoverSnippets();

        if (snippets.Count == 0)
        {
            Console.WriteLine("No snippets found.");
            return Task.CompletedTask;
        }

        Console.WriteLine($"Found {snippets.Count} snippet(s):\n");

        foreach (var snippet in snippets.OrderBy(s => s.Name))
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"  {snippet.Name}");
            Console.ResetColor();
            Console.WriteLine($" ({snippet.Language})");
            Console.WriteLine($"    Path: {snippet.RelativePath}");
        }

        return Task.CompletedTask;
    }

    private static Task ExecuteShowAsync(string name, string? source)
    {
        var templatesPath = ResolveTemplatesPath(source);
        if (templatesPath == null) return Task.CompletedTask;

        var snippetLoader = new SnippetLoader(templatesPath);
        var snippets = snippetLoader.DiscoverSnippets();

        var snippet = snippets.FirstOrDefault(s =>
            s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (snippet == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Snippet not found: {name}");
            Console.ResetColor();
            Console.WriteLine("\nAvailable snippets:");
            foreach (var s in snippets.OrderBy(x => x.Name))
            {
                Console.WriteLine($"  - {s.Name}");
            }
            return Task.CompletedTask;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Snippet: {snippet.Name}");
        Console.ResetColor();
        Console.WriteLine($"Language: {snippet.Language}");
        Console.WriteLine($"Path: {snippet.RelativePath}");
        Console.WriteLine();
        Console.WriteLine("Content:");
        Console.WriteLine(new string('-', 40));
        Console.WriteLine(snippet.Content);
        Console.WriteLine(new string('-', 40));

        return Task.CompletedTask;
    }

    private static string? ResolveTemplatesPath(string? source)
    {
        if (!string.IsNullOrEmpty(source))
        {
            if (!Directory.Exists(source))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Templates path not found: {source}");
                Console.ResetColor();
                return null;
            }
            return source;
        }

        // Try to load from project config
        var projectRoot = Directory.GetCurrentDirectory();
        var loader = new ConfigLoader();

        if (loader.HasConfiguration(projectRoot))
        {
            var config = loader.LoadConfiguration(projectRoot);
            return config.GetTemplatesPath();
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("No .modeller configuration found. Use --source to specify templates path.");
        Console.ResetColor();
        return null;
    }
}

