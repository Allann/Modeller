using Hy.Modeller.Interfaces;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils.Abstractions;
using System.IO;
using Microsoft.Extensions.Configuration;
using System;

namespace Hy.Modeller.Cli
{
    [Command(Name = "Modeller", Description = "Modeller tool is used to generate code via DLL plug-ins.")]
    [Subcommand(typeof(Build))]
    [Subcommand(typeof(Generators))]
    [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
    internal class Program
    {
        private readonly IConsole _console;
        private readonly ISettings _settings;
        private readonly CommandLineContext _context;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<Program> _logger;

        private const string _prefix = "HY_";
        private const string _appsettings = "appsettings.json";
        private const string _hostsettings = "hostsettings.json";

        public static Task<int> Main(string[] args) => new HostBuilder()
            .ConfigureHostConfiguration(configHost=> {
                configHost.SetBasePath(Directory.GetCurrentDirectory());
                configHost.AddJsonFile(_hostsettings, optional: true);
                configHost.AddEnvironmentVariables(prefix: _prefix);
            })
            .ConfigureAppConfiguration((hostContext, configApp) =>
            {
                configApp.SetBasePath(Directory.GetCurrentDirectory());
                configApp.AddJsonFile(_appsettings, optional: true);
                configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
            })
            .ConfigureLogging((context, builder) =>
            {
                if (context.HostingEnvironment.IsDevelopment())
                {
                    builder.AddDebug();
                }
            })
            .ConfigureServices((context, services) =>
            {
                services
                    .AddSingleton<ISettings, Settings>()
                    .AddSingleton(PhysicalConsole.Singleton);
            })
            .RunCommandLineApplicationAsync<Program>(args);

        public Program(IHostingEnvironment env, ILogger<Program> logger, IConsole console, ISettings settings, CommandLineContext context)
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

        internal static string GetVersion()
            => typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

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