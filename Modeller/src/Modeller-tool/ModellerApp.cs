using Hy.Modeller.Interfaces;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using McMaster.Extensions.CommandLineUtils.Abstractions;
using System;

namespace Hy.Modeller.Cli
{
    [Subcommand(typeof(Build))]
    [Subcommand(typeof(Generators))]
    internal class ModellerApp
    {
        private readonly IConsole _console;
        private readonly ISettings _settings;
        private readonly CommandLineContext _context;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<ModellerApp> _logger;
               
        public ModellerApp(IHostingEnvironment env, ILogger<ModellerApp> logger, IConsole console, ISettings settings, CommandLineContext context)
        {
            _env = env;
            _logger = logger;
            _console = console ?? NullConsole.Singleton;
            _settings = settings;
            _context = context;

            _logger.LogInformation(LoggingEvents.StartEvent, "Modeller.Start in {0} {1}", _env.EnvironmentName, _settings.SupportRegen ? "supporting regeneration" : "without regeneration");

            if (env.IsDevelopment())
            {
                Overwrite = true;
                Verbose = true;
            }
            if (!env.IsProduction())
            {
                console.WriteLine($"Environment: {env.EnvironmentName}");
            }
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

        internal int OnExecute(CommandLineApplication app)
        {
            _logger.LogInformation(LoggingEvents.ParameterEvent, "Settings: {0}, Target: {1}, Overwrite: {2}, Verbose: {3}", Settings, Target, Overwrite, Verbose);

            _console.WriteLine();
            _console.WriteLine("You need to specify a command.");
            _console.WriteLine();
            app.ShowHelp();

            if (_env.IsDevelopment())
            {
                Console.WriteLine("Press [ENTER] to finish");
                Console.ReadLine();
            }

            _logger.LogInformation(LoggingEvents.CompleteEvent, "Modeller.Complete");
            return 1;
        }
    }
}
