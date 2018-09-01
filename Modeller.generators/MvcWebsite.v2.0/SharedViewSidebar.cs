using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewSidebar : IGenerator
    {
        private readonly Module _module;

        public SharedViewSidebar(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("@model SidebarViewModel");
            sb.AppendLine();
            sb.AppendLine("@foreach (var group in Model.Items)");
            sb.AppendLine("{");
            sb.AppendLine("<div class=\"list-group\">");
            sb.AppendLine("    @foreach (var item in group.Value)");
            sb.AppendLine("    {");
            sb.AppendLine("        var enabled = item.Enabled ? \"\" : \"disabled\";");
            sb.AppendLine("        switch (item.GetType().Name)");
            sb.AppendLine("        {");
            sb.AppendLine("            case \"SidebarItem\":");
            sb.AppendLine("                <a class=\"list-group-item disabled\" id=\"@item.Id\">@item.Text</a>");
            sb.AppendLine("                break;");
            sb.AppendLine("            case \"HRefSidebarItem\":");
            sb.AppendLine("                var link = item as HRefSidebarItem;");
            sb.AppendLine("                <a class=\"list-group-item @enabled\" id=\"@item.Id\" href=\"@link.HRef\">@item.Text</a>");
            sb.AppendLine("                break;");
            sb.AppendLine("            case \"DialogSidebarItem\":");
            sb.AppendLine("                var dialog = item as DialogSidebarItem;");
            sb.AppendLine("                <a class=\"list-group-item @enabled\" id=\"@item.Id\" data-toggle=\"model\" data-target=\"#@dialog.Target\" onclick=\"javascript: @dialog.Click;\">@item.Text</a>");
            sb.AppendLine("                break;");
            sb.AppendLine("            case \"ActionSidebarItem\":");
            sb.AppendLine("                var action = item as ActionSidebarItem;");
            sb.AppendLine("                <a class=\"list-group-item @enabled\" id=\"@item.Id\" asp-controller=\"@action.Controller\" asp-action=\"@action.Action\" asp-all-route-data=\"@action.Parameters\">@item.Text</a>");
            sb.AppendLine("                break;");
            sb.AppendLine("            default:");
            sb.AppendLine("                break;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("</div>");
            sb.AppendLine("    if (Model.IncludeHorizontalLine)");
            sb.AppendLine("    {");
            sb.AppendLine("        <hr/>");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File
            {
                Name = "_Sidebar.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
