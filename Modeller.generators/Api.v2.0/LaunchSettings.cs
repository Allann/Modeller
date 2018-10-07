using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace Api
{
    internal class LaunchSettings : IGenerator
    {
        private readonly Module _module;

        public LaunchSettings(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"iisSettings\": {");
            sb.AppendLine("    \"windowsAuthentication\": false,");
            sb.AppendLine("    \"anonymousAuthentication\": true,");
            sb.AppendLine("    \"iisExpress\": {");
            sb.AppendLine("      \"applicationUrl\": \"https://localhost:44361/\",");
            sb.AppendLine("      \"sslPort\": 44361");
            sb.AppendLine("    }");
            sb.AppendLine("  },");
            sb.AppendLine("  \"profiles\": {");
            sb.AppendLine("    \"IIS Express\": {");
            sb.AppendLine("      \"commandName\": \"IISExpress\",");
            sb.AppendLine("      \"launchBrowser\": true,");
            sb.AppendLine("      \"launchUrl\": \"api-docs\",");
            sb.AppendLine("      \"environmentVariables\": {");
            sb.AppendLine("        \"ASPNETCORE_ENVIRONMENT\": \"Development\"");
            sb.AppendLine("      }");
            sb.AppendLine("    },");
            sb.AppendLine($"    \"{_module.Namespace}.Api\": {{");
            sb.AppendLine("      \"commandName\": \"Project\",");
            sb.AppendLine("      \"launchBrowser\": true,");
            sb.AppendLine("      \"environmentVariables\": {");
            sb.AppendLine("        \"ASPNETCORE_ENVIRONMENT\": \"Development\"");
            sb.AppendLine("      },");
            sb.AppendLine("      \"applicationUrl\": \"http://localhost:35836/\"");
            sb.AppendLine("    }");
            sb.AppendLine("  }");
            sb.AppendLine("}");

            return new File { Name = "launchSettings.json", Content = sb.ToString(), Path = "Properties", CanOverwrite = Settings.SupportRegen };
        }
    }
}