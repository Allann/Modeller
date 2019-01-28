using Hy.Modeller.Interfaces;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.Configuration;
using System;
using Hy.Modeller.Generator;
using Hy.Modeller.Core.Outputs;
using Serilog;
using Hy.Modeller.Outputs;

namespace Hy.Modeller.Cli
{
    [Command(Name = "Modeller", Description = "Modeller tool is used to generate code via DLL plug-ins.")]
    [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
    internal class Program
    {
        private const string _prefix = "HY_";
        private const string _appsettings = "appsettings.json";
        private const string _hostsettings = "hostsettings.json";

        internal static string GetVersion()
            => typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

        public static async Task<int> Main(string[] args)
        {
            var hostBuilder = new HostBuilder();
            try
            {
                hostBuilder
                    .ConfigureHostConfiguration(configHost =>
                    {
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
                        Log.Logger = new LoggerConfiguration()
                            .WriteTo.File("model.log")
                            .WriteTo.Console(outputTemplate: "{Message:lj}{NewLine}{Exception}")
                            .CreateLogger();
                    })
                    .ConfigureServices((context, services) =>
                    {
                        services.AddLogging(configure => configure.AddSerilog());

                        services.AddSingleton<ISettings, Settings>();
                        
                        services.AddScoped<IGeneratorConfiguration, GeneratorConfiguration>();
                        services.AddScoped<ISettingsLoader, JsonSettingsLoader>();
                        services.AddScoped<IModuleLoader, JsonModuleLoader>();
                        services.AddScoped<IGeneratorLoader, GeneratorLoader>();
                        services.AddScoped<IContext, Context>();
                        services.AddScoped<ICodeGenerator, CodeGenerator>();
                        services.AddScoped<IPresenter, Presenter>();
                        services.AddScoped<IBuilder, Outputs.Builder>();
                        services.AddScoped<IUpdater, Updater>();
                        services.AddScoped<IPackageService, PackageService>();
                        services.AddScoped<IPackageFileLoader, PackageFileLoader>();

                        services.AddTransient<IFileWriter, FileWriter>();

                        services.AddScoped<IOutputStrategy, OutputStrategy>();
                        services.AddScoped<IFileCreator, CreateFile>();
                        services.AddScoped<IFileCreator, CreateSnippet>();
                        services.AddScoped<IFileCreator, CreateProject>();
                        services.AddScoped<IFileCreator, CreateSolution>();
                        services.AddScoped<IFileCreator, FileCopier>();
                        services.AddScoped<IFileCreator, FolderCopier>();
                        services.AddScoped<IFileCreator, CreateFileGroup>();
                    });

                var host = await hostBuilder.RunCommandLineApplicationAsync<ModellerApp>(args);
                //Console.WriteLine("Press [Enter] to finish");
                //Console.ReadLine();
                return host;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Model program.cs caught an issue");
                return 1;
            }
        }
    }
}