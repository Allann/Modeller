using Hy.Modeller.Cli.Properties;
using Hy.Modeller.Generator;
using Hy.Modeller.Interfaces;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace Hy.Modeller.Cli
{
    [Command(Name = "generators", Description = "Manage generators")]
    [Subcommand(typeof(List))]
    [Subcommand(typeof(Update))]
    public class Generators
    {
        public Generators()
        {
        }

        [Command(Description = "List available generators"), HelpOption]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated via reflection")]
        private class List
        {
            private readonly IPresenter _presenter;
            private readonly ILogger<List> _logger;

            public List(IPresenter presenter, ILogger<List> logger)
            {
                _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            [Option(Description = "Path to the locally cached generators")]
            [DirectoryExists]
            public string LocalFolder { get; } = Defaults.LocalFolder;

            [Option(Description = "Target framework. Defaults to netstandard2.0", Inherited = true)]
            public string Target { get; } = Defaults.Target;

            [Option]
            public bool Verbose { get; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Top level unexpected catch all")]
            internal int OnExecute()
            {
                try
                {
                    _logger.LogTrace(Resources.ListOnExecute);

                    _presenter.GeneratorConfiguration.LocalFolder = LocalFolder;
                    _presenter.GeneratorConfiguration.Target = Target;
                    _presenter.GeneratorConfiguration.Verbose = Verbose;

                    _presenter.Display();
                    return 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError(LoggingEvents.ListError, ex, Resources.ListFailed);
                    return 1;
                }
                finally
                {
                    _logger.LogTrace(Resources.ListComplete);
                }
            }
        }

        [Command(Description = "Update generators"), HelpOption]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated via reflection")]
        private class Update
        {
            private readonly IUpdater _updater;
            private readonly ILogger<Update> _logger;

            public Update(IUpdater updater, ILogger<Update> logger)
            {
                _updater = updater ?? throw new ArgumentNullException(nameof(updater));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            [Option(Description = "Path to the locally cached generators")]
            [DirectoryExists]
            public string LocalFolder { get; } = Defaults.LocalFolder;

            [Option(Inherited = true, ShortName = "")]
            public bool Overwrite { get; }

            [Option(Description = "Server path to the host of the generators")]
            [DirectoryExists]
            [Required]
            public string ServerFolder { get; }

            [Option(Description = "Target framework. Defaults to netstandard2.0", Inherited = true)]
            public string Target { get; } = Defaults.Target;

            [Option]
            public bool Verbose { get; } = false;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Top level unexpected catch all")]
            internal int OnExecute()
            {
                _logger.LogTrace(Resources.UpdateOnExecute);
                try
                {
                    _updater.GeneratorConfiguration.LocalFolder = LocalFolder;
                    _updater.GeneratorConfiguration.Overwrite = Overwrite;
                    _updater.GeneratorConfiguration.ServerFolder = ServerFolder;
                    _updater.GeneratorConfiguration.Target = Target;
                    _updater.GeneratorConfiguration.Verbose = Verbose;

                    _updater.Refresh();
                    return 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError(LoggingEvents.UpdateError, ex, Resources.UpdateFailed);
                    return 1;
                }
                finally
                {
                    _logger.LogTrace(Resources.UpdateComplete);
                }
            }
        }

        internal int OnExecute(IConsole console, CommandLineApplication app)
        {
            console.WriteLine();
            console.WriteLine(Resources.SpecifyCommand);
            console.WriteLine();

            app.ShowHelp();

            return 1;
        }
    }
}
