using Hy.Modeller.Generator;
using Hy.Modeller.Interfaces;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Hy.Modeller.Cli
{
    [Command(Name = "generators", Description = "Manage generators")]
    [Subcommand(typeof(List))]
    [Subcommand(typeof(Update))]
    internal class Generators
    {
        //todo: Change to look like https://github.com/natemcmaster/CommandLineUtils/blob/master/docs/samples/subcommands/builder-api/Program.cs
        private readonly IHostingEnvironment _env;
        private readonly ILogger<Generators> _logger;

        public Generators(IHostingEnvironment env, ILogger<Generators> logger)
        {
            _env = env;
            _logger = logger;
        }

        [Command(Description = "List generators"), HelpOption]
        private class List
        {
            private readonly IHostingEnvironment _env;
            private readonly ILogger<Program> _logger;

            public List(IHostingEnvironment env, ILogger<Program> logger)
            {
                _env = env;
                _logger = logger;
            }

            [Option(Description = "Show all generators (default shows just for target)")]
            public bool All { get; }

            [Option(Description = "Target framework. Defaults to netstandard2.0", Inherited = true)]
            public string Target { get; } = Defaults.Target;

            [Option(Description = "Path to the locally cached generators")]
            [DirectoryExists]
            public string LocalFolder { get; } = Defaults.LocalFolder;

            [Option]
            public bool Verbose { get; }

            internal void OnExecute(IConsole console, CommandLineApplication app)
            {
                void output(string s, bool b)
                {
                    if (b)
                        console.WriteLine(s);
                    else
                        console.Write(s);
                }

                try
                {
                    var listPresenter = new Presenter(LocalFolder, Target, output);
                    listPresenter.Display(Verbose);

                    if (_env.IsDevelopment())
                    {
                        Console.WriteLine("Press [ENTER] to finish");
                        Console.ReadLine();
                    }
                }
                catch (Exception ex)
                {
                    console.WriteLine("ERROR - {0}", ex.Message);
                    _logger.LogError(LoggingEvents.ListError, ex, "List command failed. " + ex.Message);
                }
            }
        }

        [Command(Description = "Update generators"), HelpOption]
        private class Update
        {
            private readonly IHostingEnvironment _env;
            private readonly ILogger<Program> _logger;

            public Update(IHostingEnvironment env, ILogger<Program> logger)
            {
                _env = env;
                _logger = logger;
            }

            [Option(Inherited = true, ShortName = "")]
            public bool Overwrite { get; } = true;

            [Option(Description = "Target framework. Defaults to netstandard2.0", Inherited = true)]
            public string Target { get; } = Defaults.Target;

            [Option(Description = "Path to the locally cached generators")]
            [DirectoryExists]
            public string LocalFolder { get; }

            [Option(Description = "Server path to the host of the generators")]
            [DirectoryExists]
            public string ServerFolder { get; }

            [Option]
            public bool Verbose { get; } = false;

            internal void OnExecute(IConsole console, CommandLineApplication app)
            {
                try
                {
                    var updater = new Updater(server: ServerFolder, local: LocalFolder, target: Target, overwrite: Overwrite, verbose: Verbose, output: s => console.WriteLine(s));
                    updater.Refresh();

                    if (_env.IsDevelopment())
                    {
                        Console.WriteLine("Press [ENTER] to finish");
                        Console.ReadLine();
                    }
                }
                catch (Exception ex)
                {
                    console.WriteLine("ERROR - {0}", ex.Message);
                    _logger.LogError(LoggingEvents.UpdateError, ex, "Update command failed. " + ex.Message);
                }
            }
        }

        internal int OnExecute(IConsole console, CommandLineApplication app)
        {
            console.WriteLine();
            console.WriteLine("You need to specify a command.");
            console.WriteLine();

            app.ShowHelp();

            return 1;
        }
    }
}