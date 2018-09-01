using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewErrorForm : IGenerator
    {
        private readonly Module _module;

        public SharedViewErrorForm(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("@using Microsoft.AspNetCore.Html");
            sb.AppendLine("@model ErrorDialogViewModel");
            sb.AppendLine();
            sb.AppendLine("<div id=\"divErrorForm\">");
            sb.AppendLine("");
            sb.AppendLine("    <div class=\"modal-header\">");
            sb.AppendLine("        <button type=\"button\" class=\"close\" aria-label=\"Close\" data-dismiss=\"modal\">");
            sb.AppendLine("            <span aria-hidden=\"true\">&times;</span>");
            sb.AppendLine("        </button>");
            sb.AppendLine("        <h4 class=\"modal-title\" id=\"spnErrorDialogTitle\">@Model.Title</h4>");
            sb.AppendLine("    </div>");
            sb.AppendLine();
            sb.AppendLine("    <div class=\"modal-body\">");
            sb.AppendLine("        <div class=\"form-horizontal\">");
            sb.AppendLine("            <div class=\"form-group valign-center\">");
            sb.AppendLine("                <div class=\"col-md-1 jbs-dialogErrorIconWrapper\">");
            sb.AppendLine("                    <span class=\"fa fa-exclamation-triangle fa-2x\" aria-hidden=\"true\"></span>");
            sb.AppendLine("                </div>");
            sb.AppendLine("                <div class=\"col-md-11\" id=\"spnErrorDialogMessage\">");
            sb.AppendLine("                    @(new HtmlString(Model.ErrorMessage))");
            sb.AppendLine("                </div>");
            sb.AppendLine();
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");

            return new File
            {
                Name = "_ErrorForm.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
