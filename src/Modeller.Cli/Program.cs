using System.CommandLine;
using Modeller.Cli.Commands;

var rootCommand = new RootCommand("Modeller - Code generation from domain definitions");

rootCommand.AddCommand(InitCommand.Create());
rootCommand.AddCommand(GenerateCommand.Create());
rootCommand.AddCommand(ValidateCommand.Create());
rootCommand.AddCommand(TemplatesCommand.Create());
rootCommand.AddCommand(SnippetCommand.Create());

return await rootCommand.InvokeAsync(args);
