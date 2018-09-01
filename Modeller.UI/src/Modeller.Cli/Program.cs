using System;
using Core.CmdLine;
using Modeller.Generator;
using Modeller.Outputs;

namespace Modeller.Cli
{
    internal class Program
    {
        private static void Main(string[] args) => ConsoleApplication.RunProgram<ModellerCmdLine>(Start);

        private static void Start(ModellerCmdLine args)
        {
            if (args.UpdateLocal)
            {
                var updater = new Updater(server: args.ServerFolder, local: args.LocalFolder, target: args.Target, overwrite: args.Overwrite, verbose: args.Verbose, output: s => Console.WriteLine(s));
                updater.Refresh();
            }
            if (args.List)
            {
                void output(string s, bool b)
                {
                    if (b)
                        Console.WriteLine(s);
                    else
                        Console.Write(s);
                }
                var listPresenter = new Presenter(args.LocalFolder, args.Target, output);
                listPresenter.Display(args.Verbose);
            }
            if (args.UpdateLocal || args.List)
            {
                return;
            }

            var context = new Context(args.SourceModel, args.LocalFolder, args.Generator, args.Target, args.Version, args.Settings, args.ModelName, args.OutputPath);
            if (!context.IsValid)
            {
                return;
            }

            var codeGenerator = new CodeGenerator(context, s => Console.WriteLine(s), args.Verbose);
            var presenter = new Creator(context, s => Console.WriteLine(s), args.Verbose);
            presenter.Create(codeGenerator.Create());
        }
    }
}