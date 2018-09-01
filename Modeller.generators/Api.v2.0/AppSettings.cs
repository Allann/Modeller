using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace Api
{
    internal class AppSettings : IGenerator
    {
        private readonly Module _module;

        public AppSettings(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"Logging\": {");
            sb.AppendLine("    \"IncludeScopes\": false,");
            sb.AppendLine("    \"LogLevel\": {");
            sb.AppendLine("      \"Default\": \"Warning\"");
            sb.AppendLine("    }");
            sb.AppendLine("  },");
            sb.AppendLine("  \"SiteName\": \"Head Office IT Projects\",");
            sb.AppendLine("  \"SiteCode\": \"Production\",");
            sb.AppendLine("  \"appSettings\": {");
            sb.AppendLine("    \"ResponseType\": \"code id_token token\",");
            sb.AppendLine("    \"SignInScheme\": \"Bearer\"");
            sb.AppendLine("  }");
            sb.AppendLine("}");

            return new File { Name = "appsettings.json", Content = sb.ToString() };
        }
    }
}