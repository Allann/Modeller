using Hy.Modeller.Interfaces;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hy.Modeller.Cli
{
    [Subcommand(typeof(Build))]
    [Subcommand(typeof(Generators))]
    internal class ModellerApp
    {
        private readonly ISettings _settings;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<ModellerApp> _logger;

        public ModellerApp(IHostingEnvironment env, ILogger<ModellerApp> logger, ISettings settings)
        {
            _env = env;
            _logger = logger;
            _settings = settings;
        }

        [Option(Description = "Settings file to use when generating code. Settings in the file will override arguments on the command line")]
        [FileExists]
        public string Settings { get; }

        [Option(Description = "Target framework. Defaults to netstandard2.0")]
        public string Target { get; } = Defaults.Target;

        [Option(ShortName = "")]
        public bool Overwrite { get; } = true;

        [Option(ShortName = "")]
        public bool Verbose { get; }

        internal int OnExecute(IConsole console, CommandLineApplication app)
        {
            _logger.LogInformation(LoggingEvents.ParameterEvent, "Settings: {0}, Target: {1}, Overwrite: {2}, Verbose: {3}", Settings, Target, Overwrite, Verbose);

            console.WriteLine();
            console.WriteLine("You need to specify a command.");
            console.WriteLine();

            app.ShowHelp();
            
            _logger.LogInformation(LoggingEvents.CompleteEvent, "Modeller.Complete");
            return 1;
        }
    }
}
