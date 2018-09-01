using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace Api
{
    internal class AppSettingsDev : IGenerator
    {
        private readonly Module _module;

        public AppSettingsDev(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"ConnectionStrings\": {");
            sb.AppendLine($"    \"{_module.Namespace}\": \"Data Source=AUQLDRV00DEV2NI\\\\S2014S01,64001;Database={_module.Project.Singular.Value};User ID={_module.Project.Singular.Value};Password=*******;MultipleActiveResultSets=true\"");
            sb.AppendLine("  },");
            sb.AppendLine("  \"HealthCheck\": {");
            sb.AppendLine("    \"Timeout\": 1");
            sb.AppendLine("  },");
            sb.AppendLine("  \"SiteName\": \"Development\",");
            sb.AppendLine("  \"SiteCode\": \"DEV\",");
            sb.AppendLine("  \"Logging\": {");
            sb.AppendLine("    \"IncludeScopes\": false,");
            sb.AppendLine("    \"LogLevel\": {");
            sb.AppendLine("      \"Default\": \"Debug\",");
            sb.AppendLine("      \"System\": \"Information\",");
            sb.AppendLine("      \"Microsoft\": \"Information\"");
            sb.AppendLine("    }");
            sb.AppendLine("  },");
            sb.AppendLine();
            sb.AppendLine("  \"Serilog\": {");
            sb.AppendLine("    \"MinimumLevel\": {");
            sb.AppendLine("      \"Default\": \"Debug\",");
            sb.AppendLine("      \"Override\": {");
            sb.AppendLine("        \"System\": \"Information\",");
            sb.AppendLine("        \"Microsoft\": \"Information\"");
            sb.AppendLine("      }");
            sb.AppendLine("    },");
            sb.AppendLine("    \"WriteTo\": [");
            sb.AppendLine("      {");
            sb.AppendLine("        \"Name\": \"Async\",");
            sb.AppendLine("        \"Args\": {");
            sb.AppendLine("          \"configure\": [");
            sb.AppendLine("            { \"Name\": \"LiterateConsole\" },");
            sb.AppendLine("            {");
            sb.AppendLine("              \"Name\": \"Seq\",");
            sb.AppendLine("              \"Args\": {");
            sb.AppendLine("                \"serverUrl\": \"http://localhost:5341/\",");
            sb.AppendLine("                \"compact\": true");
            sb.AppendLine("              }");
            sb.AppendLine("            },");
            sb.AppendLine("            {");
            sb.AppendLine("              \"Name\": \"File\",");
            sb.AppendLine("              \"Args\": {");
            sb.AppendLine("                \"path\": \"log.txt\",");
            sb.AppendLine("                \"rollingInterval\": \"Day\",");
            sb.AppendLine("                \"retainedFileCountLimit\": 7");
            sb.AppendLine("              }");
            sb.AppendLine("            }");
            sb.AppendLine("          ]");
            sb.AppendLine("        }");
            sb.AppendLine("      }");
            sb.AppendLine("    ],");
            sb.AppendLine("    \"Enrich\": [ \"FromLogContext\" ]");
            sb.AppendLine("  },");
            sb.AppendLine();
            sb.AppendLine("  \"appSettings\": {");
            sb.AppendLine($"    \"ClientId\": \"{_module.Namespace}.Api.Development\",");
            sb.AppendLine("    \"Authority\": \"https://asap-dev.jbssa.com.au\",");
            sb.AppendLine("  }");
            sb.AppendLine("}");

            return new File { Name = "appsettings.DEV.json", Content = sb.ToString() };
        }
    }
}