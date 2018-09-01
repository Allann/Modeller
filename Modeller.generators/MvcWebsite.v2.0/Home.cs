using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class Home : IGenerator
    {
        private readonly Module _module;

        public Home(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("@{");
            sb.AppendLine("    ViewBag.Title = \"Home Page\";");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("<div class=\"container-fluid\">");
            sb.AppendLine("</div>");
            sb.AppendLine();
            sb.AppendLine("<footer class=\"footer hidden-sm hidden-xs\">");
            sb.AppendLine("    <div class=\"container-fluid\">");
            sb.AppendLine("        <small class=\"text-muted\" style=\"padding-right:60px\">JBS Australia (c) 2018</small>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</footer>");
            return new File()
            {
                Name = "Index.cshtml",
                Content = sb.ToString(),
                Path = "Home",
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}