using Hy.Modeller.Generator;
using Hy.Modeller.Generator.Outputs;
using Hy.Modeller.Interfaces;
using Hy.Modeller.Loaders;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Hy.Modeller
{
    [Command(Name = "codegen", Description = "CodeGen tool is used to generate code via DLL plug-ins.")]
    [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
    internal class Program
    {
        private const string _appsettings = "appsettings.json";
        private const string _hostsettings = "hostsettings.json";

        internal static string GetVersion()
            => typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0";

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
                            .WriteTo.File("codegen.log")
                            .WriteTo.Console(outputTemplate: "{Message:lj}{NewLine}{Exception}")
                            .CreateLogger();
                    })
                    .ConfigureServices((context, services) =>
                    {
                        services.AddLogging(configure => configure.AddSerilog());

                        services.AddSingleton<ISettings, Settings>();
                        services.AddSingleton<IGeneratorConfiguration, GeneratorConfiguration>();
                        services.AddSingleton<IContext, Context>();

                        services.AddScoped<ILoader<ISettings>, JsonSettingsLoader>();
                        services.AddScoped<ILoader<IEnumerable<INamedElement>>, JsonModuleLoader>();
                        services.AddScoped<ILoader<IEnumerable<IGeneratorItem>>, GeneratorLoader>();
                        services.AddScoped<ICodeGenerator, CodeGenerator>();
                        services.AddScoped<IPresenter, Presenter>();
                        services.AddScoped<IBuilder, Builder>();
                        services.AddScoped<IUpdater, Updater>();
                        services.AddScoped<IPackageService, PackageService>();
                        services.AddScoped<ILoader<IEnumerable<IPackage>>, PackageFileLoader>();

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

                return await hostBuilder.RunCommandLineApplicationAsync<ModellerApp>(args).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                Log.Fatal(ex, "CodeGen program.cs caught an issue");
                return 1;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}
