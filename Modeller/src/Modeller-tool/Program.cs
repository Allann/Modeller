using Hy.Modeller.Interfaces;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Hy.Modeller.Cli
{
    [Command(Name = "Modeller", Description = "Modeller tool used to generate code via DLL plug-ins.")]
    [Subcommand(typeof(Build))]
    [Subcommand(typeof(Generators))]
    [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
    internal class Program
    {
        private readonly IConsole _console;
        private readonly ISettings _settings;

        public static int Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddSingleton<ISettings, Settings>()
                .AddSingleton(PhysicalConsole.Singleton)
                .BuildServiceProvider();

            var app = new CommandLineApplication<Program>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(services);

            var result = app.Execute(args);

            Console.WriteLine();
            Console.Write("Press any key to finish.");
            Console.ReadLine();

            return result;
        }

        public Program(IConsole console, ISettings settings)
        {
            _console = console ?? NullConsole.Singleton;
            _settings = settings;
        }

        [Option(Description = "Settings file to use when generating code. Settings in the file will override arguments on the commend line", ShortName = "s")]
        [FileExists]
        public string Settings { get; }

        private static string GetVersion()
            => typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        private int OnExecute() => 0;
    }
}