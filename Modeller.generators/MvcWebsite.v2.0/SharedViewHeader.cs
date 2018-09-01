using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewHeader : IGenerator
    {
        private readonly Module _module;

        public SharedViewHeader(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<div id=\"divInnerHeaderWrapper\">");
            sb.AppendLine("    <div class=\"container-fluid\">");
            sb.AppendLine("        <header class=\"hidden-xs jbs-header\">");
            sb.AppendLine("            <div class=\"row\">");
            sb.AppendLine("                <span class=\"col-sm-3\">");
            sb.AppendLine("                    <a href=\"http://jbsintranet.amh.com.au\">");
            sb.AppendLine("                        <img src=\"~/images/JBSlogo.jpg\" alt=\"Jbs Logo\" style=\"width:120px; height:40px;\" />");
            sb.AppendLine("                    </a>");
            sb.AppendLine("                </span>");
            sb.AppendLine("                <div class=\"col-sm-6\">");
            sb.AppendLine("                    @await Component.InvokeAsync(\"Title\")");
            sb.AppendLine("                </div>");
            sb.AppendLine("                <div class=\"col-sm-3\">");
            sb.AppendLine("                    @await Component.InvokeAsync(\"User\")");
            sb.AppendLine("                </div>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </header>");
            sb.AppendLine("    </div>");
            sb.AppendLine("    <div id=\"divJBS-navbar\" class=\"navbar navbar-default navbar-static-top\" role=\"navigation\" data-spy=\"affix\" data-offset-top=\"50\">");
            sb.AppendLine("        <a class=\"navbar-toggle\" data-toggle=\"collapse\" data-target=\".navbar-collapse\">");
            sb.AppendLine("            <span class=\"sr-only\">Toggle navigation</span>");
            sb.AppendLine("            <span class=\"icon-bar\"></span>");
            sb.AppendLine("            <span class=\"icon-bar\"></span>");
            sb.AppendLine("            <span class=\"icon-bar\"></span>");
            sb.AppendLine("        </a>");
            sb.AppendLine("        <div class=\"collapse navbar-collapse\">");
            sb.AppendLine("            <ul class=\"nav navbar-nav\">");
            sb.AppendLine($"                <li class=\"\"><a class=\"navbar-brand\" href=\"/\"><i class=\"fas fa-home\"></i> {_module.Project.Singular.Display}</a></li>");
            sb.AppendLine("            </ul>");
            sb.AppendLine("            <partial name=\"_Menu\"/>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");
            return new File
            {
                Name = "_Header.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
