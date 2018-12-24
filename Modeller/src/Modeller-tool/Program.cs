using Hy.Modeller.Interfaces;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.IO;
using Microsoft.Extensions.Configuration;
using System;

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
                        if (context.HostingEnvironment.IsDevelopment())
                        {
                            builder.AddDebug();
                        }
                    })
                    .ConfigureServices((context, services) =>
                    {
                        services.AddSingleton<ISettings, Settings>();
                    });

                return await hostBuilder.RunCommandLineApplicationAsync<ModellerApp>(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 1;
            }
        }
    }
}