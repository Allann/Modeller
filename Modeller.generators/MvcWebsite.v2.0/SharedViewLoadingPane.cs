using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewLoadingPane : IGenerator
    {
        private readonly Module _module;
        
        public SharedViewLoadingPane(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("@model LoadingPaneViewModel");
            sb.AppendLine();
            sb.AppendLine("<div id=\"@Model.DivId\" style=\"display: none;\">");
            sb.AppendLine("    <div class=\"jbs-overlay\">");
            sb.AppendLine("        <div class=\"text-center jbs-loader\">");
            sb.AppendLine("            <i class=\"fas fa-spinner fa-spin\"></i><span class=\"jbs-loadingPaneText\">@Model.LoadingText</span>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");
            return new File
            {
                Name = "_LoadingPane.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
