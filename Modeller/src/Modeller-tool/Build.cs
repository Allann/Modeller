using McMaster.Extensions.CommandLineUtils;

namespace Hy.Modeller.Cli
{
    [Command(Name = "build", Description = "Use DLL components to generate code")]
    internal class Build
    {
        [Argument(0, Description = "The generator to use.")]
        public string Generator { get; }

        [Argument(1, Description = "The filename for the source model to use during code generation.")]
        [FileExists]
        public string SourceModel { get; }

        [Option(Description = "Output folder", ShortName = "o")]
        [DirectoryExists]
        public string Output { get; }

        [Option(Description = "Model name. If included then the output will be limited to the specified model")]
        public string Model { get; }

        private int OnExecute(IConsole console)
        {
            console.Error.WriteLine("You must specify an action. See --help for more details.");
            return 1;
        }
    }
}