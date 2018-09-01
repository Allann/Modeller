using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewLayout : IGenerator
    {
        private readonly Module _module;
        
        public SharedViewLayout(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("@{");
            sb.AppendLine("    var culture = System.Globalization.CultureInfo.CurrentCulture.ToString();");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset=\"utf-8\" />");
            sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" />");
            sb.AppendLine("    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">");
            sb.AppendLine("    <title>@await Component.InvokeAsync(\"PageTitle\")</title>");
            sb.AppendLine("    <link rel=\"icon\" type=\"image/x-icon\" href=\"~/favicon.ico\" />");
            sb.AppendLine("    <link rel=\"shortcut icon\" type=\"image/x-icon\" href=\"~/favicon.ico\" />");
            sb.AppendLine();
            sb.AppendLine("    <environment include=\"Development\">");
            sb.AppendLine("        <link rel=\"stylesheet\" href=\"~/bundle/kendo.min.css\" asp-append-version=\"true\" />");
            sb.AppendLine("        <link rel=\"stylesheet\" href=\"~/lib/font-awesome/web-fonts-with-css/css/fontawesome-all.css\" asp-append-version=\"true\"/>");
            sb.AppendLine("        <script src=\"~/bundle/jquery.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("        <script src=\"~/bundle/kendo.all.min.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("        <script src=\"~/bundle/kendo.aspnetmvc.min.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("        <script src=\"~/lib/kendo-ui/js/cultures/kendo.culture.@{@culture}.min.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("        <link rel=\"stylesheet\" href=\"~/bundle/bootstrap.css\" asp-append-version=\"true\" />");
            sb.AppendLine("        <link rel=\"stylesheet\" href=\"~/bundle/jquery-ui.css\" asp-append-version=\"true\" />");
            sb.AppendLine("        <link rel=\"stylesheet\" href=\"~/bundle/kendo-bootstrap.min.css\" asp-append-version=\"true\" />");
            sb.AppendLine("        <link rel=\"stylesheet\" href=\"~/bundle/toastr.css\" asp-append-version=\"true\" />");
            sb.AppendLine("        <link rel=\"stylesheet\" href=\"~/bundle/site.css\" asp-append-version=\"true\" />");
            sb.AppendLine("    </environment>");
            sb.AppendLine();
            sb.AppendLine("    <environment exclude=\"Development\">");
            sb.AppendLine("        <link rel=\"stylesheet\" href=\"~/bundle/kendo.min.css\" asp-append-version=\"true\" />");
            sb.AppendLine("        <link rel=\"stylesheet\" href=\"~/lib/font-awesome/web-fonts-with-css/css/fontawesome-all.min.css\" asp-append-version=\"true\" />");
            sb.AppendLine("        <script src=\"~/bundle/jquery.min.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("        <script src=\"~/bundle/jquery-ui.min.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("        <script src=\"~/bundle/kendo.all.min.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("        <script src=\"~/bundle/kendo.aspnetmvc.min.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("        <script src=\"~/lib/kendo-ui/js/cultures/kendo.culture.@{@culture}.min.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("        <link rel=\"stylesheet\" href=\"~/bundle/bootstrap.min.css\" asp-append-version=\"true\" />");
            sb.AppendLine("        <link rel=\"stylesheet\" href=\"~/bundle/jquery-ui.min.css\" asp-append-version=\"true\" />");
            sb.AppendLine("        <link rel=\"stylesheet\" href=\"~/bundle/kendo-bootstrap.min.css\" asp-append-version=\"true\" />");
            sb.AppendLine("        <link rel=\"stylesheet\" href=\"~/bundle/toastr.min.css\" asp-append-version=\"true\" />");
            sb.AppendLine("        <link rel=\"stylesheet\" href=\"~/bundle/site.min.css\" asp-append-version=\"true\" />");
            sb.AppendLine("    </environment>");
            sb.AppendLine("</head>");
            sb.AppendLine();
            sb.AppendLine("<body>");
            sb.AppendLine("    <partial name=\"_ToastrBuilder\"/>");
            sb.AppendLine();
            sb.AppendLine("    <div class=\"body-content\">");
            sb.AppendLine("        <div id=\"divHeaderWrapper\">");
            sb.AppendLine("            <partial name=\"_Header\"/>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div class=\"clearfix\">");
            sb.AppendLine("            <!-- fix for IE -->");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <div id=\"divBodyWrapper\">");
            sb.AppendLine("            @{ Html.RenderPartial(\"_LoadingPane\", new LoadingPaneViewModel { DivId = \"divRootLoadingPane\" }); }");
            sb.AppendLine("            @RenderBody()");
            sb.AppendLine("            <a href=\"#\" id=\"scroll\" class=\"jbs-scroll\" style=\"display: none;\"><span></span></a>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine();
            sb.AppendLine("    <environment include=\"Development\">");
            sb.AppendLine("        <script src=\"~/bundle/bootstrap.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("        <script src=\"~/bundle/jquery-ui.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("        <script src=\"~/bundle/jquery.validate.min.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("        <script src=\"~/bundle/toastr.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("        <script src=\"~/bundle/site.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("    </environment>");
            sb.AppendLine();
            sb.AppendLine("    <environment exclude=\"Development\">");
            sb.AppendLine("        <script src=\"~/bundle/bootstrap.min.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("        <script src=\"~/bundle/jquery-ui.min.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("        <script src=\"~/bundle/jquery.validate.min.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("        <script src=\"~/bundle/toastr.min.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("        <script src=\"~/bundle/site.min.js\" asp-append-version=\"true\"></script>");
            sb.AppendLine("    </environment>");
            sb.AppendLine();
            sb.AppendLine("    @Html.Kendo().DeferredScripts()");
            sb.AppendLine("    @RenderSection(\"scripts\", required: false)");
            sb.AppendLine("    <script type=\"text/javascript\">");
            sb.AppendLine("      kendo.culture('@culture');");
            sb.AppendLine();
            sb.AppendLine("        $.validator.setDefaults({");
            sb.AppendLine("            ignore: \"\"");
            sb.AppendLine("        });");
            sb.AppendLine();
            sb.AppendLine("        $(document).ajaxError(function (event, jqxhr, settings, exception) {");
            sb.AppendLine("            console.log(\"AJAX error in request: \" + JSON.stringify(jqxhr.error(), null, 2));");
            sb.AppendLine("            if (jqxhr.statusText !== \"abort\") {");
            sb.AppendLine("                alert(\"An unexpected error has occurred, if the problem persists please contact the IT Helpdesk.\");");
            sb.AppendLine("            }");
            sb.AppendLine("        });");
            sb.AppendLine();
            sb.AppendLine("        window.addEventListener('error', function (e) {");
            sb.AppendLine("            alert(\"An unexpected error has occurred, if the problem persists please contact the IT Helpdesk.\");");
            sb.AppendLine("        });");
            sb.AppendLine("    </script>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");


            return new File
            {
                Name = "_Layout.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
