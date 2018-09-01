using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewModelAccessDenied : IGenerator
    {
        private readonly Module _module;
        
        public SharedViewModelAccessDenied(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"namespace {_module.Namespace}.Web.ViewModels.Shared");
            sb.AppendLine("{");
            sb.AppendLine("    public class AccessDeniedViewModel");
            sb.AppendLine("    {");
            sb.AppendLine("        public string ReturnUrl { get; set; }");
            sb.AppendLine();
            sb.AppendLine("        public string AccessArea { get { return ReturnUrl.Replace(\"/\", \" \").Trim(); } }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File
            {
                Name = "AccessDeniedViewModel.cs",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
