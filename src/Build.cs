using Hy.Modeller.Base.Models;
using Hy.Modeller.Interfaces;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;

namespace Hy.Modeller.Cli
{
    [Command(Name = "build", Description = "Use DLL components to generate code")]
    internal class Build
    {
        private readonly IBuilder _builder;
        private readonly ILogger<Program> _logger;

        public Build(IBuilder builder, ILogger<Program> logger)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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
        public string Version { get; } = "1.0.0";

        [Option(ShortName = "", Inherited = true)]
        public bool Verbose { get; }

        internal int OnExecute()
        {
            try
            {
                _logger.LogTrace("Generator Build Command - OnExecute");

                _builder.Context.GeneratorConfiguration.GeneratorName = Generator;
                _builder.Context.GeneratorConfiguration.LocalFolder = LocalFolder;
                _builder.Context.GeneratorConfiguration.ModelName = Model;
                _builder.Context.GeneratorConfiguration.OutputPath = Output;
                _builder.Context.GeneratorConfiguration.SettingsFile = Settings;
                _builder.Context.GeneratorConfiguration.SourceModel = SourceModel;
                _builder.Context.GeneratorConfiguration.Target = Target;
                _builder.Context.GeneratorConfiguration.Verbose = Verbose;
                _builder.Context.GeneratorConfiguration.Version = new GeneratorVersion(Version);

                _builder.Create();

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.BuildError, ex, "Build command failed. " + ex.Message);
                return 1;
            }
            finally
            {
                _logger.LogTrace("Generator Build Command - complete");
            }
        }
    }
}