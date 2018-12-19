using Hy.Modeller.Generator;
using Hy.Modeller.Outputs;
using McMaster.Extensions.CommandLineUtils;
using System.Text;

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

        [Option(Description = "Path to the locally cached generators")]
        [DirectoryExists]
        public string LocalFolder { get; } = Defaults.LocalFolder;

        [Option(Description = "Model name. If included then the output will be limited to the specified model")]
        public string Model { get; }

        [Option(Description = "Output folder")]
        [DirectoryExists]
        public string Output { get; } = Defaults.OutputFolder;

        [Option(Inherited = true, ShortName = "")]
        public bool Overwrite { get; } = true;

        [Option(Description = "Target framework. Defaults to netstandard2.0", Inherited = true)]
        public string Target { get; } = Defaults.Target;

        [Option(Description = "Settings file to use when generating code. Settings in the file will override arguments on the command line", ShortName = "s", Inherited = true)]
        [FileExists]
        public string Settings { get; }

        [Option(Description = "Specific version to use for the generator", ShortName = "")]
        public string Version { get; } = Defaults.Version.ToString();

        [Option(ShortName = "", Inherited = true)]
        public bool Verbose { get; }

        internal int OnExecute(IConsole console)
        {
            var context = new Context(SourceModel, LocalFolder, Generator, Target, Version, Settings, Model, Output, output: s => console.WriteLine(s));
            var codeGenerator = new CodeGenerator(context, s => console.WriteLine(s), true);
            var presenter = new Creator(context, s => console.WriteLine(s), true);
            presenter.Create(codeGenerator.Create());
            return 0;
        }
    }
}