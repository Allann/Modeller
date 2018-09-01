using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewQuickFilter : IGenerator
    {
        private readonly Module _module;

        public SharedViewQuickFilter(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("@model BaseQuickFilterViewModel");
            sb.AppendLine();
            sb.AppendLine("@{");
            sb.AppendLine("    if (Model.ShowAdvancedSearch)");
            sb.AppendLine("    {");
            sb.AppendLine("        <div class=\"well jbs-quickFilter-panel\">");
            sb.AppendLine("            <form class=\"form-inline\" id=\"formQuickSearch\">");
            sb.AppendLine();
            sb.AppendLine("                <input type=\"hidden\" id=\"KeywordFilter\" name=\"KeywordFilter\" value=\"@Model.KeywordFilter\" />");
            sb.AppendLine("                <input type=\"hidden\" id=\"QuickFilter\" name=\"QuickFilter\" value=\"@Model.QuickFilter\" />");
            sb.AppendLine();
            sb.AppendLine("                <partial name=\"_Keyword\" />");
            sb.AppendLine("                @{if (Model.ShowCheckbox)");
            sb.AppendLine("                    {");
            sb.AppendLine("                        <div class=\"form-group jbs-isActiveFilter\">");
            sb.AppendLine("                            <input id=\"filterIsActive\" name=\"ShowInActive\" type=\"checkbox\" data-onstyle=\"default\" data-toggle=\"toggle\" />");
            sb.AppendLine("                            <label for=\"filterIsActive\" >Show Inactive</label>");
            sb.AppendLine("                        </div>");
            sb.AppendLine("                    }}");
            sb.AppendLine("                <div class=\"form-group pull-right\">");
            sb.AppendLine("                    <a class=\"btn btn-default\" data-toggle=\"collapse\" href=\"#divAdvancedSearch\">Advanced Search</a>");
            sb.AppendLine("                </div>");
            sb.AppendLine("            </form>");
            sb.AppendLine();
            sb.AppendLine("            <div id=\"divAdvancedSearch\" class=\"collapse\">");
            sb.AppendLine("                <div>");
            sb.AppendLine("                    <partial name=\"_AdvancedFilter\" />");
            sb.AppendLine("                </div>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    }");
            sb.AppendLine("    else");
            sb.AppendLine("    {");
            sb.AppendLine("        <div class=\"well jbs-quickFilter-panel\">");
            sb.AppendLine("            <form class=\"form-inline\" id=\"formQuickSearch\">");
            sb.AppendLine();
            sb.AppendLine("                <input type=\"hidden\" id=\"KeywordFilter\" name=\"KeywordFilter\" value=\"@Model.KeywordFilter\" />");
            sb.AppendLine("                <input type=\"hidden\" id=\"QuickFilter\" name=\"QuickFilter\" value=\"@Model.QuickFilter\" />");
            sb.AppendLine();
            sb.AppendLine("                <partial name=\"_Keyword\" />");
            sb.AppendLine("                @{ if (Model.ShowCheckbox)");
            sb.AppendLine("                    {");
            sb.AppendLine("                        <div class='form-group jbs-isActiveFilter'>");
            sb.AppendLine("                            <input id=\"filterIsActive\" name=\"ShowInActive\" type=\"checkbox\" data-onstyle=\"default\" data-toggle=\"toggle\" />");
            sb.AppendLine("                            <label for=\"filterIsActive\" >Show Inactive</label>");
            sb.AppendLine("                        </div>");
            sb.AppendLine("                    }");
            sb.AppendLine("                }");
            sb.AppendLine("            </form>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            sb.AppendLine("<script type=\"text/javascript\">");
            sb.AppendLine("    if ('@Model.ShowInActive' === 'True') {");
            sb.AppendLine("        $(\"#filterIsActive\").attr(\"checked\", \"checked\");");
            sb.AppendLine("    }");
            sb.AppendLine("</script>");

            return new File
            {
                Name = "_QuickFilter.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}