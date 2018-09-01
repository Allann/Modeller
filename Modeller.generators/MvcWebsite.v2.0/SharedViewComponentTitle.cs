using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewComponentTitle : IGenerator
    {
        private readonly Module _module;
        
        public SharedViewComponentTitle(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("@model ApplicationModel");
            sb.AppendLine();
            sb.AppendLine("<div style=\"display: table; position: absolute; height: 100%; width: 100%\">");
            sb.AppendLine("    <div style=\"display: table-cell;vertical-align:middle\">");
            sb.AppendLine("        <div class=\"text-center\" style=\"margin-left: auto; margin-right: auto;\">");
            sb.AppendLine("            <h4 style=\"margin: 0\">@Model.ApplicationName @await Component.InvokeAsync(\"Version\")</h4>");
            sb.AppendLine("            <div style=\"font-weight: 100\">@Model.SiteName</div>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");

            return new File
            {
                Name = "Default.cshtml",
                Path = System.IO.Path.Combine("Shared","Components", "Title"),
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}