using System.CommandLine;
using System.Reflection;
using Modeller.Cli.Commands;

var version = Assembly.GetExecutingAssembly()
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
    ?? Assembly.GetExecutingAssembly().GetName().Version?.ToString()
    ?? "unknown";

var rootCommand = new RootCommand($"Modeller v{version} - Code generation from domain definitions");

rootCommand.Subcommands.Add(InitCommand.Create());
rootCommand.Subcommands.Add(GenerateCommand.Create());
rootCommand.Subcommands.Add(ValidateCommand.Create());
rootCommand.Subcommands.Add(TemplatesCommand.Create());
rootCommand.Subcommands.Add(SnippetCommand.Create());

return await rootCommand.Parse(args).InvokeAsync();
