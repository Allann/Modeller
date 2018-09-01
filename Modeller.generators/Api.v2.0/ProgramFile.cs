using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace Api
{
    internal class ProgramFile : IGenerator
    {
        private readonly Module _module;

        public ProgramFile(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.IO;");
            sb.AppendLine("using Microsoft.AspNetCore;");
            sb.AppendLine("using Microsoft.AspNetCore.Hosting;");
            sb.AppendLine("using Microsoft.Extensions.Configuration;");
            sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
            sb.AppendLine("using Microsoft.Extensions.Logging;");
            sb.AppendLine($"using {_module.Namespace}.Api;");
            sb.AppendLine($"using {_module.Namespace}.Data;");
            sb.AppendLine("using Serilog;");
            sb.AppendLine("using Jbssa.Core.HealthChecks.AspNetCore;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Api");
            sb.AppendLine("{");
            sb.AppendLine("    public class Program");
            sb.AppendLine("    {");
            sb.AppendLine("        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()");
            sb.AppendLine("            .SetBasePath(Directory.GetCurrentDirectory())");
            sb.AppendLine("            .AddJsonFile(\"appsettings.json\", optional: false, reloadOnChange: true)");
            sb.AppendLine("            .AddJsonFile($\"appsettings.{Environment.GetEnvironmentVariable(\"ASPNETCORE_ENVIRONMENT\") ?? \"Production\"}.json\", optional: true)");
            sb.AppendLine("            .AddEnvironmentVariables()");
            sb.AppendLine("            .Build();");
            sb.AppendLine();
            sb.AppendLine("        public static int Main(string[] args)");
            sb.AppendLine("        {");
            sb.AppendLine("            Log.Logger = new LoggerConfiguration()");
            sb.AppendLine("                .ReadFrom.Configuration(Configuration)");
            sb.AppendLine("                .Enrich.FromLogContext()");
            sb.AppendLine("                .WriteTo.Console()");
            sb.AppendLine("                .CreateLogger();");
            sb.AppendLine();
            sb.AppendLine("            try");
            sb.AppendLine("            {");
            sb.AppendLine("                Log.Information(\"Starting host\");");
            sb.AppendLine();
            sb.AppendLine("                var host = BuildWebHost(args);");
            sb.AppendLine("                ProcessDbCommands(args, host);");
            sb.AppendLine("                host.Run();");
            sb.AppendLine("                return 0;");
            sb.AppendLine("            }");
            sb.AppendLine("            catch (Exception ex)");
            sb.AppendLine("            {");
            sb.AppendLine("                Log.Fatal(ex, \"Host terminated unexpectedly\");");
            sb.AppendLine("                return 1;");
            sb.AppendLine("            }");
            sb.AppendLine("            finally");
            sb.AppendLine("            {");
            sb.AppendLine("                Log.CloseAndFlush();");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public static IWebHost BuildWebHost(string[] args) =>");
            sb.AppendLine("            WebHost.CreateDefaultBuilder(args)");
            sb.AppendLine("                .ConfigureLogging(builder =>");
            sb.AppendLine("                {");
            sb.AppendLine("                    builder.ClearProviders();");
            sb.AppendLine("                    builder.AddSerilog();");
            sb.AppendLine("                })");
            sb.AppendLine("                .UseHealthChecks(\"/hc\")");
            sb.AppendLine("                .UseStartup<Startup>()");
            sb.AppendLine("                .UseConfiguration(Configuration)");
            sb.AppendLine("                .Build();");
            sb.AppendLine();
            sb.AppendLine("        private static void ProcessDbCommands(string[] args, IWebHost host)");
            sb.AppendLine("        {");
            sb.AppendLine("            var services = (IServiceScopeFactory)host.Services.GetService(typeof(IServiceScopeFactory));");
            sb.AppendLine("            using (var scope = services.CreateScope())");
            sb.AppendLine("            {");
            sb.AppendLine("                if (args.Contains(\"dropdb\"))");
            sb.AppendLine("                {");
            sb.AppendLine("                    Console.WriteLine(\"Dropping database\");");
            sb.AppendLine("                    var db = GetDb(scope);");
            sb.AppendLine("                    db.Database.EnsureDeleted();");
            sb.AppendLine("                }");
            sb.AppendLine("                if (args.Contains(\"migratedb\"))");
            sb.AppendLine("                {");
            sb.AppendLine("                    Console.WriteLine(\"Performing database migrations\");");
            sb.AppendLine("                    var db = GetDb(scope);");
            sb.AppendLine("                    db.Database.Migrate();");
            sb.AppendLine("                }");
            sb.AppendLine("                //if (args.Contains(\"seeddb\"))");
            sb.AppendLine("                //{");
            sb.AppendLine("                //    Console.WriteLine(\"Seeding database\");");
            sb.AppendLine("                //    SeedData.Initialize(scope.ServiceProvider);");
            sb.AppendLine("                //}");
            sb.AppendLine("            }");
            sb.AppendLine("            if (args.Contains(\"stop\"))");
            sb.AppendLine("            {");
            sb.AppendLine("                Console.WriteLine(\"exiting on stop command\");");
            sb.AppendLine("                Environment.Exit(0);");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        private static {_module.Project.Singular.Value}DbContext GetDb(IServiceScope services)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var db = services.ServiceProvider.GetRequiredService<{_module.Project.Singular.Value}DbContext>();");
            sb.AppendLine("            return db;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File { Name = "Program.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}