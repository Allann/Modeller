using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewDetailHeader : IGenerator
    {
        private readonly Module _module;

        public SharedViewDetailHeader(ISettings settings, Module module)
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
            //sb.AppendLine("            <a authorize jbs-auth-resource=\"@Model.Authorization.ResourceName\" jbs-auth-permission=\"Create\" class=\"btn btn-outline btn-default\" asp-action=\"@Model.CreateAction\"><i class=\"fas fa-plus\"></i> Create New</a>");
            //sb.AppendLine("            <a authorize jbs-auth-resource=\"@Model.Authorization.ResourceName\" jbs-auth-permission=\"Update\" class=\"btn btn-outline btn-default\" asp-action=\"@Model.EditAction\" asp-route-id=\"@Model.BusinessKey\"><i class=\"far fa-edit\"></i> Edit</a>");
            sb.AppendLine("            <a class=\"btn btn-outline btn-default\" asp-action=\"@Model.CreateAction\"><i class=\"fas fa-plus\"></i> Create New</a>");
            sb.AppendLine("            <a class=\"btn btn-outline btn-default\" asp-action=\"@Model.EditAction\" asp-route-id=\"@Model.BusinessKey\"><i class=\"far fa-edit\"></i> Edit</a>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");

            return new File
            {
                Name = "_DetailHeader.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}