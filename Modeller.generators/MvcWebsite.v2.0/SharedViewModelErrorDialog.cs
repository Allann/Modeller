using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewModelErrorDialog : IGenerator
    {
        private readonly Module _module;
        
        public SharedViewModelErrorDialog(ISettings settings, Module module)
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
            sb.AppendLine("    public class ErrorDialogViewModel");
            sb.AppendLine("    {");
            sb.AppendLine("        public string Title { get; set; }");
            sb.AppendLine();
            sb.AppendLine("        public string ErrorMessage { get; set; }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File
            {
                Name = "ErrorDialogViewModel.cs",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
