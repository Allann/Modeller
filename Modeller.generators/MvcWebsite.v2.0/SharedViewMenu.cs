using System;
using System.Linq;
using Modeller;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewMenu : IGenerator
    {
        private readonly Module _module;

        public SharedViewMenu(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<ul class=\"nav navbar-nav\">");
            sb.AppendLine("    <li class=\"dropdown\" style=\"display:none\">");
            sb.AppendLine("        <a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\">Menu<span class=\"caret\"></span></a>");
            sb.AppendLine("        <ul class=\"dropdown-menu\" role=\"menu\" aria-labelledby=\"dropdownMenu\">");
            foreach (var model in _module.Models.Where(m => m.IsEntity()))
            {
                //sb.AppendLine($"            <li authorize jbs-auth-resource=\"SupportedResources.{model.Name.Singular.Value}\" jbs-auth-permission=\"Read\"><a asp-controller=\"{model.Name.Plural.Value}\" asp-action=\"Index\">{model.Name.Singular.Value}</a></li>");
                sb.AppendLine($"            <li><a asp-controller=\"{model.Name.Plural.Value}\" asp-action=\"Index\">{model.Name.Singular.Value}</a></li>");
            }
            sb.AppendLine("        </ul>");
            sb.AppendLine("    </li>");
            sb.AppendLine("</ul>");
            sb.AppendLine();
            sb.AppendLine("@*<ul class=\"nav navbar-top-links navbar-right\">");
            sb.AppendLine("    <!--Alerts-->");
            sb.AppendLine("    <li authorize jbs-auth-resource=\"SupportedResources.Hangfire\" jbs-auth-permission=\"Read\" class=\"dropdown pull-left\">");
            sb.AppendLine("        <a class=\"dropdown-toggle\" data-toggle=\"dropdown\" href=\"/\">");
            sb.AppendLine("            <i class=\"fas fa-bell\"></i> <i class=\"fas fa-caret-down\"></i>");
            sb.AppendLine("        </a>");
            sb.AppendLine("        <ul authorize jbs-auth-resource=\"SupportedResources.Hangfire\" jbs-auth-permission=\"Execute\" class=\"dropdown-menu dropdown-alerts\">");
            sb.AppendLine("            <li><a href=\"/hangfire\">Scheduled Tasks</a></li>");
            sb.AppendLine("        </ul>");
            sb.AppendLine("    </li>");
            sb.AppendLine("    <!--Alerts-->");
            sb.AppendLine("    <!--Settings-->");
            sb.AppendLine("    <li class=\"dropdown pull-left\">");
            sb.AppendLine("        <a class=\"dropdown-toggle\" data-toggle=\"dropdown\" href=\"#\">");
            sb.AppendLine("            <i class=\"fas fa-cog\"></i> <i class=\"fas fa-caret-down\"></i>");
            sb.AppendLine("        </a>");
            sb.AppendLine("        <ul class=\"dropdown-menu dropdown-user\">");
            sb.AppendLine("            <li><a asp-controller=\"Options\" asp-action=\"Index\">Options</a></li>");
            sb.AppendLine("        </ul>");
            sb.AppendLine("    </li>");
            sb.AppendLine("    <!--Settings-->");
            sb.AppendLine("    <!--User-->");
            sb.AppendLine("    <li class=\"dropdown pull-left\">");
            sb.AppendLine("        <a class=\"dropdown-toggle\" data-toggle=\"dropdown\" href=\"#\">");
            sb.AppendLine("            <i class=\"fas fa-user\"></i> <i class=\"fas fa-caret-down\"></i>");
            sb.AppendLine("        </a>");
            sb.AppendLine("        <ul class=\"dropdown-menu dropdown-user\">");
            sb.AppendLine("            <li><a asp-controller=\"User\" asp-action=\"MyDetails\">My details</a></li>");
            sb.AppendLine("        </ul>");
            sb.AppendLine("    </li>");
            sb.AppendLine("    <!--User-->");
            sb.AppendLine("</ul>*@");
            sb.AppendLine();
            sb.AppendLine("<script>");
            sb.AppendLine("    $(document).ready(function () {");
            sb.AppendLine("        $('ul:not([showChildren])').each(function () {");
            sb.AppendLine("            if ($(this).has('li').length) $(this).parent('li').css('display', 'block');");
            sb.AppendLine("        });");
            sb.AppendLine("    });");
            sb.AppendLine("</script>");
            return new File
            {
                Name = "_Menu.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }

}
