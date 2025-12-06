using System.CommandLine;
using System.Reflection;
using Modeller.Cli.Commands;

var version = Assembly.GetExecutingAssembly()
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
    ?? Assembly.GetExecutingAssembly().GetName().Version?.ToString()
    ?? "unknown";

var rootCommand = new RootCommand($"Modeller v{version} - Code generation from domain definitions");

rootCommand.AddCommand(InitCommand.Create());
rootCommand.AddCommand(GenerateCommand.Create());
rootCommand.AddCommand(ValidateCommand.Create());
rootCommand.AddCommand(TemplatesCommand.Create());
rootCommand.AddCommand(SnippetCommand.Create());

return await rootCommand.InvokeAsync(args);
