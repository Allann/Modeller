using System.CommandLine;
using Modeller.Cli.Commands;

var rootCommand = new RootCommand("Modeller - Code generation from domain definitions");

rootCommand.AddCommand(InitCommand.Create());
rootCommand.AddCommand(GenerateCommand.Create());
rootCommand.AddCommand(TemplatesCommand.Create());

return await rootCommand.InvokeAsync(args);
