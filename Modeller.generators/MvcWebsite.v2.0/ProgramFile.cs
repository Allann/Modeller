using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
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
            sb.AppendLine("using Jbssa.Core.HealthChecks.AspNetCore;");
            sb.AppendLine($"using {_module.Namespace}.Web;");
            sb.AppendLine("using Microsoft.AspNetCore;");
            sb.AppendLine("using Microsoft.AspNetCore.Hosting;");
            sb.AppendLine("using Microsoft.Extensions.Configuration;");
            sb.AppendLine("using Microsoft.Extensions.Logging;");
            sb.AppendLine("using Newtonsoft.Json;");
            sb.AppendLine("using Serilog;");
            sb.AppendLine("using Serilog.Sinks.SystemConsole.Themes;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Project.Singular.Value}.Web");
            sb.AppendLine("{");
            sb.AppendLine("    public class Program");
            sb.AppendLine("    {");
            sb.AppendLine("        private static IConfiguration _configuration;");
            sb.AppendLine();
            sb.AppendLine("        private class JbsEnvironment");
            sb.AppendLine("        {");
            sb.AppendLine("            public string Name { get; set; }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public static IConfiguration Configuration");
            sb.AppendLine("        {");
            sb.AppendLine("            get");
            sb.AppendLine("            {");
            sb.AppendLine("                if (_configuration != null)");
            sb.AppendLine("                    return _configuration;");
            sb.AppendLine();
            sb.AppendLine("                var env = GetEnvironment();");
            sb.AppendLine("                if (env == null)");
            sb.AppendLine("                {");
            sb.AppendLine("                    env = new JbsEnvironment");
            sb.AppendLine("                    {");
            sb.AppendLine("                        Name = Environment.GetEnvironmentVariable(\"ASPNETCORE_ENVIRONMENT\")");
            sb.AppendLine("                    };");
            sb.AppendLine("                }");
            sb.AppendLine();
            sb.AppendLine("                _configuration = new ConfigurationBuilder()");
            sb.AppendLine("                    .SetBasePath(Directory.GetCurrentDirectory())");
            sb.AppendLine("                    .AddJsonFile(\"appsettings.json\", optional: false, reloadOnChange: true)");
            sb.AppendLine("                    .AddJsonFile($\"appsettings.{Environment.GetEnvironmentVariable(\"ASPNETCORE_ENVIRONMENT\") ?? \"Production\"}.json\", optional: true)");
            sb.AppendLine("                    .AddEnvironmentVariables()");
            sb.AppendLine("                    .Build();");
            sb.AppendLine("                return _configuration;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private static JbsEnvironment GetEnvironment()");
            sb.AppendLine("        {");
            sb.AppendLine("            if (!File.Exists(\"environment.json\"))");
            sb.AppendLine("                return null;");
            sb.AppendLine("            var lines = File.ReadAllLines(\"environment.json\");");
            sb.AppendLine("            var json = string.Concat(lines);");
            sb.AppendLine("            return JsonConvert.DeserializeObject<JbsEnvironment>(json);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public static int Main(string[] args)");
            sb.AppendLine("        {");
            sb.AppendLine($"            Console.Title = \"JBS Australia {_module.Project.Singular.Value}\";");
            sb.AppendLine();
            sb.AppendLine("            Log.Logger = new LoggerConfiguration()");
            sb.AppendLine("                .ReadFrom.Configuration(Configuration)");
            sb.AppendLine("                .Enrich.FromLogContext()");
            sb.AppendLine("                .WriteTo.Console(outputTemplate: \"[{ Timestamp: HH: mm: ss} {Level}] {SourceContext} {NewLine} {Message: lj}{NewLine}{Exception}{NewLine}\", theme: AnsiConsoleTheme.Literate)");
            sb.AppendLine("                .CreateLogger();");
            sb.AppendLine();
            sb.AppendLine("            try");
            sb.AppendLine("            {");
            sb.AppendLine("                Log.Information(\"Starting web host\");");
            sb.AppendLine("                BuildWebHost(args).Run();");
            sb.AppendLine("                return 0;");
            sb.AppendLine("            }");
            sb.AppendLine("            catch (Exception ex)");
            sb.AppendLine("            {");
            sb.AppendLine("                Log.Fatal(ex, \"Web Host terminated unexpectedly\");");
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
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File { Name = "Program.cs", Content = sb.ToString() };
        }
    }
}