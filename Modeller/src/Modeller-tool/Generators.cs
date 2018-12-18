using Hy.Modeller.Generator;
using McMaster.Extensions.CommandLineUtils;

namespace Hy.Modeller.Cli
{
    [Command(Name = "generators", Description = "Manage generators")]
    [Subcommand(typeof(List))]
    [Subcommand(typeof(Update))]
    internal class Generators
    {
        [Command(Description = "List generators"), HelpOption]
        private class List
        {
            [Option(Description = "Show all generators (default shows just for target)")]
            public bool All { get; }

            [Option(Description = "Target framework. Defaults to netstandard2.0")]
            public string Target { get; } = "netstandard2.0";

            [Option(Description = "Path to the locally cached generators")]
            [DirectoryExists]
            public string LocalFolder { get; }

            [Option]
            public bool Verbose { get; } = false;

            private void OnExecute(IConsole console, CommandLineApplication app)
            {
                void output(string s, bool b)
                {
                    if (b)
                        console.WriteLine(s);
                    else
                        console.Write(s);
                }

                var listPresenter = new Presenter(LocalFolder, Target, output);
                listPresenter.Display(Verbose);
            }
        }

        [Command(Description = "Update generators"), HelpOption]
        private class Update
        {
            [Option]
            public bool Overwrite { get; } = true;

            [Option(Description = "Target framework. Defaults to netstandard2.0")]
            public string Target { get; } = "netstandard2.0";

            [Option(Description = "Path to the locally cached generators")]
            [DirectoryExists]
            public string LocalFolder { get; }

            [Option(Description = "Server path to the host of the generators")]
            [DirectoryExists]
            public string ServerFolder { get; }

            [Option]
            public bool Verbose { get; } = false;

            private void OnExecute(IConsole console, CommandLineApplication app)
            {
                var updater = new Updater(server: ServerFolder, local: LocalFolder, target: Target, overwrite: Overwrite, verbose: Verbose, output: s => console.WriteLine(s));
                updater.Refresh();
            }
        }
    }
}