using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewFooter : IGenerator
    {
        private readonly Module _module;

        public SharedViewFooter(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("@model FooterViewModel");
            sb.AppendLine();
            sb.AppendLine("<footer class=\"footer hidden-sm hidden-xs\">");
            sb.AppendLine("    <div class=\"container-fluid\">");
            sb.AppendLine("        <small class=\"text-muted\" style=\"float:left;\">@Model.Left</small>");
            sb.AppendLine("        <small class=\"text-muted\" style=\"float:right;\">@Model.Right</small>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</footer>");

            return new File
            {
                Name = "_Footer.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
