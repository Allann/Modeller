using Hy.Modeller.Cli;
using Hy.Modeller.Cli.Properties;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;

namespace Hy.Modeller
{
    [Subcommand(typeof(Build))]
    [Subcommand(typeof(Generators))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated via reflection")]
    internal class ModellerApp
    {
        private readonly ILogger<ModellerApp> _logger;

        public ModellerApp(ILogger<ModellerApp> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Option(Description = "Settings file to use when generating code. Settings in the file will override arguments on the command line")]
        [FileExists]
        public string Settings { get; }

        [Option(Description = "Target framework. Defaults to netstandard2.0")]
        public string Target { get; } = Generator.Defaults.Target;

        [Option(ShortName = "")]
        public bool Overwrite { get; } = true;

        [Option(ShortName = "")]
        public bool Verbose { get; }

        internal int OnExecute(IConsole console, CommandLineApplication app)
        {
            _logger.LogInformation(LoggingEvents.ParameterEvent, Resources.ParameterEvent, Settings, Target, Overwrite, Verbose);

            console.WriteLine();
            console.WriteLine("You need to specify a command.");
            console.WriteLine();

            app.ShowHelp();

            _logger.LogInformation(LoggingEvents.CompleteEvent, Resources.CompleteEvent);
            return 1;
        }
    }
}
