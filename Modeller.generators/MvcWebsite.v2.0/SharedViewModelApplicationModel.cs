using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewModelApplicationModel : IGenerator
    {
        private readonly Module _module;
        
        public SharedViewModelApplicationModel(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("using Microsoft.AspNetCore.Hosting;");
            sb.AppendLine("using Microsoft.Extensions.Configuration;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Web.ViewModels.Shared");
            sb.AppendLine("{");
            sb.AppendLine("    public class ApplicationModel");
            sb.AppendLine("    {");
            sb.AppendLine("        public ApplicationModel(IHostingEnvironment hostingEnvironment, IConfiguration configuration)");
            sb.AppendLine("        {");
            sb.AppendLine("            Version = Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion;");
            sb.AppendLine("            EnvironmentName = hostingEnvironment.EnvironmentName;");
            sb.AppendLine("            SiteName = configuration.GetValue(typeof(string), \"SiteName\")?.ToString();");
            sb.AppendLine("            SiteCode = configuration.GetValue(typeof(string), \"SiteCode\")?.ToString();");
            sb.AppendLine("        }");
            sb.AppendLine("");
            sb.AppendLine("        public string Version { get; }");
            sb.AppendLine($"        public string ApplicationName {{ get; }} = \"{_module.Project.Singular.Display}\";");
            sb.AppendLine("        public string EnvironmentName { get; }");
            sb.AppendLine("        public string SiteName { get; }");
            sb.AppendLine("        public string SiteCode { get; }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File
            {
                Name = "ApplicationModel.cs",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
