using System;
using System.Text;
using Modeller;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebViews
{
    internal class IndexView : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public IndexView(ISettings settings, Module module, Model model)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"@using {_module.Namespace}.Web.ViewModels.{_model.Name.Plural.Value}");
            sb.AppendLine($"@model {_model.Name.Singular.Value}FilterViewModel");
            sb.AppendLine();
            sb.AppendLine("@{");
            sb.AppendLine("    ViewData[\"Title\"] = Model.Page.Title;");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("<div id=\"sidebarWrapper\">");
            sb.AppendLine("    <div id=\"divJBS-Index\" class=\"col-md-10 col-md-push-2\">");
            sb.AppendLine("        <partial name=\"_IndexHeader\" for=\"Page\" />");
            sb.AppendLine("        <partial name=\"_QuickFilter\" />");
            sb.AppendLine();

            var grid = new Grid(Settings, _module, _model)
            {
                DataBound = "dataBound",
                Change = "change"
            };
            var snippet = (ISnippet)grid.Create();
            sb.Append(snippet.Content);

            sb.AppendLine("    </div>");
            sb.AppendLine();
            sb.AppendLine("    <div id=\"dvSidebarResults\" class=\"jbs-sidenav col-md-2 col-md-pull-10\" data-spy=\"affix\" data-offset-top=\"50\">");
            sb.AppendLine("        <partial name=\"_Sidebar\" for=\"Page.Sidebar\" />");
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<partial name=\"_Footer\" for=\"Page.Footer\" />");
            sb.AppendLine();
            sb.AppendLine("@section Scripts {");
            sb.AppendLine("    <script type=\"text/javascript\">");
            sb.AppendLine("        function change(e) {");
            sb.AppendLine("            selectedData = getSelectedEntity(e, this);");
            sb.AppendLine("            if (!selectedData) return;");
            sb.AppendLine("            var data = JSON.stringify(selectedData);");
            sb.AppendLine("            $.ajax({");
            sb.AppendLine($"                url: '@(Url.Action(\"SidebarItems\",\"{_model.Name.Plural.Value}\"))',");
            sb.AppendLine("                method: \"POST\",");
            sb.AppendLine("                data: { selectedData: data },");
            sb.AppendLine("                cache: false");
            sb.AppendLine("            })");
            sb.AppendLine("                .done(function (view) {");
            sb.AppendLine("                    $(\"#dvSidebarResults\").html(view);");
            sb.AppendLine("                });");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        function dataBound(e) {");
            sb.AppendLine($"            bindDblClick(this, '@Url.Action(\"GotoDefaultView\", KnownApiRoutes.{_model.Name.Plural.Value}.Name)', 'kendoListGrid', 'Id');");
            sb.AppendLine("        }");
            var bk = _model.HasBusinessKey();
            if (bk != null)
            {
                sb.AppendLine();
                sb.AppendLine($"        function {_model.Name.Singular.LocalVariable}{bk.Name.Singular.Value}Template(data) {{");
                sb.AppendLine($"            return actionLink('@Url.Action(\"GotoDefaultView\")', data.{_model.Key.Fields[0].Name.Singular.Value}, data.{bk.Name.Singular.Value});");
                sb.AppendLine("        }");
            }
            sb.AppendLine("    </script>");
            sb.AppendLine("}");
            return new File { Name = "Index.cshtml", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}