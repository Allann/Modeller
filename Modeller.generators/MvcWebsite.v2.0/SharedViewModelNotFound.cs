using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewModelNotFound : IGenerator
    {
        private readonly Module _module;
        
        public SharedViewModelNotFound(ISettings settings, Module module)
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
            sb.AppendLine("    public class NotFoundViewModel");
            sb.AppendLine("    {");
            sb.AppendLine("        public string MailTo { get; set; }");
            sb.AppendLine();
            sb.AppendLine("        public string Body { get; set; }");
            sb.AppendLine();
            sb.AppendLine("        public string PreviousUrl { get; set; }");
            sb.AppendLine();
            sb.AppendLine("        public bool AllowEmail { get; set; }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File
            {
                Name = "NotFoundViewModel.cs",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
