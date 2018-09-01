using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewValidationSummary : IGenerator
    {
        private readonly Module _module;
        
        public SharedViewValidationSummary(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("@if (ViewContext.ModelState.IsValid == false)");
            sb.AppendLine("{");
            sb.AppendLine("    <div class=\"alert alert-danger\">");
            sb.AppendLine("        <strong>Error</strong>");
            sb.AppendLine("        <div asp-validation-summary=\"All\" class=\"danger\"></div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("}");

            return new File
            {
                Name = "_ValidationSummary.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
