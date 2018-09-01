using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modeller;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebViews
{
    internal class ChildView : IGenerator
    {
        private readonly Module _module;
        private readonly Model _parent;
        private readonly Model _child;
        private readonly Name _childKey;

        public ChildView(ISettings settings, Module module, Model parent, Model child, Name childKey)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _child = child ?? throw new ArgumentNullException(nameof(child));
            _childKey = childKey ?? throw new ArgumentNullException(nameof(childKey));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            if (!_child.IsEntity())
                return null;

            var sb = new StringBuilder();
            sb.AppendLine($"@using {_module.Namespace}.Web.ViewModels.{_child.Name.Plural.Value}");
            sb.AppendLine($"@model {_child.Name.Singular.Value}FilterViewModel");
            sb.AppendLine();
            sb.AppendLine("@{");
            sb.AppendLine("    ViewBag.Title = Model.Page.Title;");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("<div id=\"sidebarWrapper\">");
            sb.AppendLine("    <div id=\"divJBS-Index\" class=\"col-md-10 col-md-push-2\">");
            sb.AppendLine("        <div class=\"clearfix\">");
            sb.AppendLine("            <div class=\"pull-left\">");
            sb.AppendLine("                <h2>@Model.Page.Title</h2>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div authorize jbs-auth-resource=\"@Model.Page.Authorization.ResourceName\" jbs-auth-permission=\"Create\" class=\"jbs-action-buttonbar\">");
            sb.AppendLine("                <div class=\"buttons-wrap\">");
            sb.AppendLine($"                    <a class=\"btn btn-outline btn-default\" asp-action=\"@Model.Page.CreateAction\" asp-route-{_childKey.Singular.LocalVariable}=\"@Model.{_childKey.Singular.Value}\"><i class=\"fas fa-plus\"></i> Create New</a>");
            sb.AppendLine("                </div>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");
            sb.AppendLine();
            sb.AppendLine($"        <partial name=\"_{_parent.Name.Singular.Value}Info\" for=\"{_parent.Name.Singular.Value}\" />");
            sb.AppendLine($"        <input asp-for=\"{_childKey.Singular.Value}\" type=\"hidden\" />");
            sb.AppendLine();

            var filters = new List<string>
            {
                $"<datasource-filter field=\"{_childKey.Singular.Value}\" operator=\"eq\" value=\"@Model.{_childKey.Singular.Value}\" />"
            };
            var grid = new Grid(Settings, _module, _child)
            {
                Change = "change",
                DataBound = "dataBound"
            };
            var snippet = (ISnippet)grid.Create();
            sb.AppendLine(snippet.Content);

            sb.AppendLine("    </div>");
            sb.AppendLine();
            sb.AppendLine("    <div id=\"dvSidebarResults\" class=\"jbs-sidenav col-md-2 col-md-pull-10\" data-spy=\"affix\" data-offset-top=\"50\">");
            sb.AppendLine("        <partial name=\"_Sidebar\" for=\"Page.Sidebar\" />");
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<partial name=\"_Footer\" for=\"Page.Footer\" />");
            sb.AppendLine("");
            sb.AppendLine("@section scripts {");
            sb.AppendLine("    <script type=\"text/javascript\">");
            sb.AppendLine("        function change(e) {");
            sb.AppendLine("            selectedData = getSelectedEntity(e, this);");
            sb.AppendLine("            if (!selectedData) return;");
            sb.AppendLine("            var data = JSON.stringify(selectedData);");
            sb.AppendLine($"            var applicationId = $('#{_childKey.Singular.Value}').val();");
            sb.AppendLine("            $.ajax({");
            sb.AppendLine($"                url: '@(Url.Action(\"SidebarItems\",\"{_child.Name.Plural.Value}\"))',");
            sb.AppendLine("                method: \"POST\",");
            sb.AppendLine($"                data: {{ selectedData: data, {_childKey.Singular.LocalVariable}: {_childKey.Singular.LocalVariable} }},");
            sb.AppendLine("                cache: false");
            sb.AppendLine("            })");
            sb.AppendLine("                .done(function (view) {");
            sb.AppendLine("                    $(\"#dvSidebarResults\").html(view);");
            sb.AppendLine("                })");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        function dataBound(e) {");
            sb.AppendLine($"            bindDblClick(this, '@Url.Action(\"GotoDefaultView\", KnownApiRoutes.{_child.Name.Plural.Value}.Name)', 'kendoListGrid', '{_child.Key.Fields.First().Name.Singular.Value}');");
            sb.AppendLine("        }");
            sb.AppendLine();
            var bk = _child.HasBusinessKey();
            if (bk != null)
            {
                sb.AppendLine($"        function {_child.Name.Singular.LocalVariable}{bk.Name.Singular.Value}Template(data) {{");
                sb.AppendLine($"            return actionLink('@Url.Action(\"GotoDefaultView\")', data.{_child.Key.Fields.First().Name.Singular.Value}, data.{bk.Name.Singular.Value});");
                sb.AppendLine("        }");
            }
            sb.AppendLine("    </script>");
            sb.AppendLine("}");

            var file = new File
            {
                Name = $"For{_parent.Name.Singular.Value}.cshtml",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
            return file;
        }
    }
}