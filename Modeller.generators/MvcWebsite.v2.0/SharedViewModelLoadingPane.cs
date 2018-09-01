using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewModelLoadingPane : IGenerator
    {
        private readonly Module _module;
        
        public SharedViewModelLoadingPane(ISettings settings, Module module)
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
            sb.AppendLine("    public class LoadingPaneViewModel");
            sb.AppendLine("    {");
            sb.AppendLine("        public LoadingPaneViewModel()");
            sb.AppendLine("        {");
            sb.AppendLine("            DivId = \"divLoadingPane\";");
            sb.AppendLine("            LoadingText = \"Loading...\";");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public string DivId { get; set; }");
            sb.AppendLine();
            sb.AppendLine("        public string LoadingText { get; set; }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File
            {
                Name = "LoadingPaneViewModel.cs",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
