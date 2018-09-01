using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
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

            sb.AppendLine("using Jbssa.Core.Security;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Web");
            sb.AppendLine("{");
            sb.AppendLine("    public class AppSettings");
            sb.AppendLine("    {");
            sb.AppendLine($"        public string {_module.Project.Singular.Value}BaseUri {{ get; set; }}");
            sb.AppendLine("        public string SiteName { get; set; }");
            sb.AppendLine("        public string SiteCode { get; set; }");
            sb.AppendLine("        public bool UseResilientHttp { get; set; }");
            sb.AppendLine("        public int HttpClientRetryCount { get; set; }");
            sb.AppendLine("        public int HttpClientExceptionsAllowedBeforeBreaking { get; set; }");
            sb.AppendLine("        public HealthCheck HealthCheck { get; set; }");
            sb.AppendLine("        public Logging Logging { get; set; }");
            sb.AppendLine("        public SecuritySettings Security { get; set; }");
            sb.AppendLine();
            sb.AppendLine("        public string HelpdeskEmail { get; set; } = \"servicedesk@jbssa.com.au\";");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    public class HealthCheck");
            sb.AppendLine("    {");
            sb.AppendLine("        public int Timeout { get; set; }");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    public class Logging");
            sb.AppendLine("    {");
            sb.AppendLine("        public bool IncludeScopes { get; set; }");
            sb.AppendLine("        public Loglevel LogLevel { get; set; }");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    public class Loglevel");
            sb.AppendLine("    {");
            sb.AppendLine("        public string Default { get; set; }");
            sb.AppendLine("        public string System { get; set; }");
            sb.AppendLine("        public string Microsoft { get; set; }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return new File() { Name = "AppSettings.cs", Content = sb.ToString()};
        }
    }
}