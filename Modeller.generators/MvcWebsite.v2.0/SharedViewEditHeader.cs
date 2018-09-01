using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewEditHeader : IGenerator
    {
        private readonly Module _module;

        public SharedViewEditHeader(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("@model PageViewModel");
            sb.AppendLine("");
            sb.AppendLine("<div class=\"clearfix\">");
            sb.AppendLine("    <div class=\"pull-left\">");
            sb.AppendLine("        <h2 class=\"pull-left\">@Model.Title</h2>");
            sb.AppendLine("        <span class=\"pull-left\" id=\"spnDetailsHeader\"></span>");
            sb.AppendLine("    </div>");
            sb.AppendLine("    <div class=\"jbs-action-buttonbar\">");
            sb.AppendLine("        <div class=\"buttons-wrap\">");
            sb.AppendLine("            <a class=\"btn btn-outline btn-default\" asp-action=\"Index\"><i class=\"fas fa-arrow-left\"></i> Back to List</a>");
            sb.AppendLine("            <button type=\"submit\" class=\"btn btn-outline btn-default\" id=\"detailForm-BtnSave\"><i class=\"far fa-save\"></i> Save</button>");
            sb.AppendLine("            @if (string.IsNullOrWhiteSpace(Model.Referer))");
            sb.AppendLine("            {");
            sb.AppendLine("                <a class=\"btn btn-outline btn-default\" asp-action=\"Detail\" asp-route-id=\"@Model.BusinessKey\" id=\"detailForm-BtnCancel\"><i class=\"far fa-times-circle\"></i> Cancel</a>");
            sb.AppendLine("            }");
            sb.AppendLine("            else");
            sb.AppendLine("            {");
            sb.AppendLine("                <a class=\"btn btn-outline btn-default\" href=\"@Model.Referer\" id=\"detailForm-BtnCancel\"><i class=\"far fa-times-circle\"></i> Cancel</a>");
            sb.AppendLine("            }");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");

            return new File
            {
                Name = "_EditHeader.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
