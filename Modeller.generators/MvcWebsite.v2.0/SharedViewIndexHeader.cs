using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewIndexHeader : IGenerator
    {
        private readonly Module _module;

        public SharedViewIndexHeader(ISettings settings, Module module)
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
            sb.AppendLine("    </div>");
            //sb.AppendLine("    <div authorize jbs-auth-resource=\"@Model.Authorization.ResourceName\" jbs-auth-permission=\"Create\" class=\"jbs-action-buttonbar\">");
            sb.AppendLine("    <div class=\"jbs-action-buttonbar\">");
            sb.AppendLine("        <div class=\"buttons-wrap\">");
            sb.AppendLine("            <a class=\"btn btn-outline btn-default\" asp-action=\"@Model.CreateAction\"><i class=\"fas fa-plus\"></i> Create New</a>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");

            return new File
            {
                Name = "_IndexHeader.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}