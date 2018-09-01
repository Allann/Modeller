using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewValidationScriptsPartial : IGenerator
    {
        private readonly Module _module;
        
        public SharedViewValidationScriptsPartial(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("<environment names=\"Development\">");
            sb.AppendLine("    <script src=\"~/bundle/jquery.validate.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("</environment>");
            sb.AppendLine("<environment names=\"Staging, Prodution\">");
            sb.AppendLine("    <script src=\"~/bundle/jquery.validate.min.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("</environment>");

            return new File
            {
                Name = "_ValidationScriptsPartial.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
