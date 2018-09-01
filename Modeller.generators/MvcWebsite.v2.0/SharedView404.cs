using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedView404 : IGenerator
    {
        private readonly Module _module;
        
        public SharedView404(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("@model NotFoundViewModel");
            sb.AppendLine("@{");
            sb.AppendLine("    ViewBag.Title = \"404 Page\";");
            sb.AppendLine("}");
            sb.AppendLine("@{ Layout = \"_Layout\"; }");
            sb.AppendLine();
            sb.AppendLine("<link rel=\"stylesheet\" href=\"~/css/cows.css\" asp-append-version=\"true\" />");
            sb.AppendLine("<div>");
            sb.AppendLine("    <div class=\"mooo\">");
            sb.AppendLine("        <div class=\"cow\">");
            sb.AppendLine("            <div class=\"cow-head\">");
            sb.AppendLine("                <div class=\"cow-head-pattern\"><div><div></div></div></div>");
            sb.AppendLine("                <div class=\"cow-nose\"><div><div></div></div></div>");
            sb.AppendLine("                <div class=\"cow-nose-bottom\"><div class=\"moo-mouth\"></div></div>");
            sb.AppendLine("                <div class=\"cow-grass\"></div>");
            sb.AppendLine("                <div class=\"cow-ear1\"></div><div class=\"cow-ear2\"></div>");
            sb.AppendLine("                <div class=\"cow-horn\"></div>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div class=\"cow-body-shell\">");
            sb.AppendLine("                <div class=\"cow-body\"></div>");
            sb.AppendLine("                <div class=\"cow-forelegs\"></div>");
            sb.AppendLine("                <div class=\"cow-hindlegs\"></div>");
            sb.AppendLine("                <div class=\"cow-tail\"></div>");
            sb.AppendLine("                <div class=\"cow-udder\"><div></div></div>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <h1>404</h1>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<br />");
            sb.AppendLine("<div>");
            sb.AppendLine("    <p style=\"width: 100%; text-align: center\"><font size=\"22\"><b>PAGE NOT FOUND! MOOO-ve On!!</b></font></p>");
            sb.AppendLine("    <p style=\"width: 100%; text-align: center\">@Model.Body<b>@Model.PreviousUrl</b></p>");
            sb.AppendLine("    <p style=\"width: 100%; text-align: center\">Hit the Home button below or choose another option from the menu.</p>");
            sb.AppendLine("    <br />");
            sb.AppendLine("    <p style=\"text-align: center\">");
            sb.AppendLine("        <a class=\"btn btn-outline btn-default\" asp-controller=\"Home\" asp-action=\"Index\" style=\"padding-left: 5em; padding-right: 5em\"><i class=\"fa fa-home\"></i> Home</a>");
            sb.AppendLine("        @if(Model.AllowEmail){");
            sb.AppendLine("            <a class=\"btn btn-outline btn-default\" href=\"@Model.MailTo\" style=\"padding-left: 5em; padding-right: 5em\"><i class=\"fa fa-envelope\"></i> Helpdesk</a>");
            sb.AppendLine("        }");
            sb.AppendLine("    </p>");
            sb.AppendLine("</div>");
            sb.AppendLine();
            sb.AppendLine("<style id=\"extraClass\">");
            sb.AppendLine("    .extraClassAspect {");
            sb.AppendLine("        -webkit-transform: scaleX(1) !important;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    .extraClassCrop {");
            sb.AppendLine("        -webkit-transform: scale(1) !important;");
            sb.AppendLine("    }");
            sb.AppendLine("</style>");

            return new File
            {
                Name = "404.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}