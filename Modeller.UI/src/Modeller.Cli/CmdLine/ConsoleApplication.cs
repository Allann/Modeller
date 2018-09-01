using System;
using System.IO;

namespace Core.CmdLine
{
    /// <summary>
    /// Delegate method that is used to run a console application.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args"></param>
    public delegate void Run<in T>(T args) where T : ICmdLineObject;

    /// <summary>
    /// Provides helper methods for command-line applications.
    /// </summary>
    public static class ConsoleApplication
    {
        /// <summary>
        /// Convenient method for running an application. Provides command-line argument initialization, help text, error handling, and waiting for exit. This is typically the only line of code in Main.
        /// </summary>
        /// <typeparam name="TArgs">The type for the CmdLineObject to use. Must have a default constructor.</typeparam>
        /// <param name="run">The method to run once the arguments are initialized. Will not be called if the help flag is set or the args aren't valid.</param>
        public static void RunProgram<TArgs>(Run<TArgs> run) where TArgs : ICmdLineObject, new()
        {
            var args = Activator.CreateInstance<TArgs>();
            args.Initialize();

            try
            {
                // Validate only after checking to see if they requested help
                // in order to prevent displaying errors when they request help.
                if (args.Help || !args.IsValid())
                {
                    // Subtract 1 from the console width so that we can prevent
                    // end-of-line issues.
                    Console.WriteLine(args.GetHelpText(GetWindowWidth()));
                    Environment.ExitCode = -1;
                    return;
                }

                run(args);
            }
            catch (Exception ex)
            {
                WriteError(ex);
                Environment.ExitCode = 1;
            }
            finally
            {
                if (args.Options.Wait)
                {
                    Console.WriteLine();
                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                }
            }
        }

        private static int GetWindowWidth()
        {
            //this check is done to allow us to redirect the output handle to a TextWriter
            //without getting an invalid handle
            //this is done by having our unit tests set the width before it runs
            try
            {
                return Console.WindowWidth;
            }
            catch (IOException) //if the output is redirected to a stream then getting the width will fail
            {
                return 80;
            }
        }

        /// <summary>
        /// Displays the exception to the console.
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteError(Exception ex)
        {
            var curEx = ex;
            if (curEx == null)
                return;
            Console.WriteLine("ERROR!!!");
            while (curEx != null)
            {
                Console.WriteLine(new string('*', GetWindowWidth()));
                Console.WriteLine(string.Format("{0} - {1}", curEx.GetType().FullName, curEx.Message));
                Console.WriteLine(curEx.StackTrace);
                Console.WriteLine();
                curEx = curEx.InnerException;
            }
        }
    }
}