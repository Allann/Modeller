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
    internal class DetailView : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public DetailView(ISettings settings, Module module, Model model)
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
            sb.AppendLine($"@model {_model.Name.Singular.Value}DetailViewModel");
            sb.AppendLine();
            sb.AppendLine("@{");
            sb.AppendLine("    ViewBag.Title = Model.Page.Title;");
            sb.AppendLine("}");
            sb.AppendLine("<partial name=\"_DeleteDialog\" for=\"Page.DeleteDialog\" />");
            sb.AppendLine();
            sb.AppendLine("<div id=\"sidebarWrapper\">");
            sb.AppendLine("    <div class=\"col-md-10 col-md-push-2\">");
            sb.AppendLine("        <form asp-action=\"Detail\" id=\"formDetail\">");
            sb.AppendLine("            <partial name=\"_DetailHeader\" for=\"Page\" />");
            foreach (var field in _model.Key.Fields)
            {
                sb.AppendLine($"            <input asp-for=\"{field.Name.Singular.Value}\" type=\"hidden\" />");
            }
            sb.AppendLine();
            sb.AppendLine("            <div class=\"form-horizontal\">");
            foreach (var field in _model.Fields)
            {
                sb.AppendLine("                <div class=\"form-group\">");
                switch (field.DataType)
                {
                    case DataTypes.UniqueIdentifier:
                        foreach (var item in _model.Relationships)
                        {
                            item.GetMatch(_model.Name, out var matchType, out var matchField);
                            item.GetOther(_model.Name, out var otherType, out var otherModel, out var otherField);
                            if (!field.Name.Equals(matchField))
                                continue;

                            if (matchType == RelationShipTypes.Many)
                            {
                                var om = _module.Models.Single(m => m.Name.Equals(otherModel));
                                var bk = om.HasBusinessKey();
                                if (bk != null)
                                {
                                    sb.AppendLine($"                    <label asp-for=\"{om.Name.Singular.Value}{bk.Name.Singular.Value}\" class=\"col-md-2 control-label\"></label>");
                                    sb.AppendLine("                    <div class=\"col-md-10\">");
                                    sb.AppendLine($"                        <input asp-for=\"{om.Name.Singular.Value}{bk.Name.Singular.Value}\" class=\"form-control\" autocomplete=\"off\" readonly />");
                                    sb.AppendLine("                    </div>");
                                }
                                else
                                {
                                    sb.AppendLine($"                    <label asp-for=\"{field.Name.Singular.Value}\" class=\"col-md-2 control-label\"></label>");
                                    sb.AppendLine("                    <div class=\"col-md-10\">");
                                    sb.AppendLine($"                        <input asp-for=\"{field.Name.Singular.Value}\" class=\"form-control\" autocomplete=\"off\" readonly />");
                                    sb.AppendLine("                    </div>");
                                }
                                break;
                            }
                        }

                        break;

                    case DataTypes.Bool:
                        sb.AppendLine($"                    <label asp-for=\"{field.Name.Singular.Value}\" class=\"col-md-2 control-label\"></label>");
                        sb.AppendLine("                    <div class=\"col-md-10\">");
                        if (field.Name.Singular.Value == "IsActive")
                        {
                            sb.AppendLine($"                        <input id=\"{field.Name.Singular.LocalVariable}\" asp-for=\"{field.Name.Singular.Value}\" type=\"checkbox\" data-on=\"Yes\" data-off=\"No\" data-offstyle=\"warning\" data-onstyle=\"default\" data-toggle=\"toggle\" disabled />");
                        }
                        else
                        {
                            sb.AppendLine($"                        <input id=\"{field.Name.Singular.LocalVariable}\" asp-for=\"{field.Name.Singular.Value}\" type=\"checkbox\" data-onstyle=\"default\" data-toggle=\"toggle\" disabled />");
                        }
                        sb.AppendLine("                    </div>");

                        break;

                    case DataTypes.Date:
                    default:
                        sb.AppendLine($"                    <label asp-for=\"{field.Name.Singular.Value}\" class=\"col-md-2 control-label\"></label>");
                        sb.AppendLine("                    <div class=\"col-md-10\">");
                        sb.AppendLine($"                        <input asp-for=\"{field.Name.Singular.Value}\" class=\"form-control\" readonly />");
                        sb.AppendLine("                    </div>");
                        break;
                }
                sb.AppendLine("                </div>");
            }

            var first = true;
            var addedTab = false;
            foreach (var relationship in _model.Relationships)
            {
                relationship.GetOther(_model.Name, out var type, out var model, out var field);
                if (type == RelationShipTypes.Many)
                {
                    addedTab = true;
                    var active = first ? " class=\"active\"" : string.Empty;
                    if (first)
                    {
                        sb.AppendLine("                <hr />");
                        sb.AppendLine("                <div id=\"tabContainer\">");
                        sb.AppendLine("                    <ul class=\"nav nav-tabs\">");
                    }
                    sb.AppendLine($"                        <li{active}><a id=\"{relationship.RightModel.Plural.LocalVariable}\" href=\"#{relationship.RightModel.Singular.LocalVariable}Pane\" data-toggle=\"tab\">Has these {relationship.RightModel.Plural.Display}</a></li>");
                    first = false;
                }
            }
            if (addedTab)
            {
                sb.AppendLine("                    </ul>");
                sb.AppendLine("                    <div class=\"tab-content\">");
                first = true;
                foreach (var relationship in _model.Relationships)
                {
                    relationship.GetMatch(_model.Name, out var matchType, out var matchField);
                    relationship.GetOther(_model.Name, out var type, out var model, out var field);
                    if (type == RelationShipTypes.Many)
                    {
                        var otherModel = _module.Models.SingleOrDefault(m => m.Name.Equals(model));
                        if (otherModel == null || !otherModel.IsEntity())
                            continue;

                        var active = first ? " active" : string.Empty;
                        first = false;
                        sb.AppendLine($"                        <div class=\"tab-pane{active}\" id=\"{model.Singular.LocalVariable}Pane\">");
                        var filters = new List<string>() { $"<datasource-filter field=\"{field.Singular.Value}\" operator=\"eq\" value=\"@Model.Id\" />" };
                        var grid = new Grid(Settings, _module, otherModel, 7, model.Singular.LocalVariable + "Grid", filters)
                        {
                            DataBound = model.Singular.LocalVariable + "DataBound"
                        };
                        var snippet = (ISnippet)grid.Create();
                        sb.Append(snippet.Content);
                        sb.AppendLine("                        </div>");
                    }
                }
                sb.AppendLine("                    </div>");
                sb.AppendLine("                </div>");
                sb.AppendLine("            </div>");
            }
            sb.AppendLine("            <partial name=\"_DeleteButton\" for=\"Page\" />");
            sb.AppendLine("        </form>");
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
            if (_model.Fields.Any(f => f.DataType == DataTypes.Bool))
            {
                sb.AppendLine("        $(function() {");
                foreach (var field in _model.Fields.Where(f => f.DataType == DataTypes.Bool))
                {
                    sb.AppendLine($"            $('#{field.Name.Singular.LocalVariable}').bootstrapToggle();");
                }
                sb.AppendLine("        })");
            }
            foreach (var relationship in _model.Relationships)
            {
                relationship.GetOther(_model.Name, out var type, out var model, out var field);
                if (type == RelationShipTypes.Many)
                {
                    var otherModel = _module.Models.SingleOrDefault(m => m.Name.Equals(model));
                    if (otherModel == null || !otherModel.IsEntity())
                        continue;

                    sb.AppendLine($"        function {model.Singular.LocalVariable}DataBound(e) {{");
                    sb.AppendLine($"            bindDblClick(this, '@Url.Action(\"GotoDefaultView\", KnownApiRoutes.{relationship.RightModel.Plural.Value}.Name)', '{relationship.RightModel.Singular.LocalVariable}Grid', 'Id');");
                    sb.AppendLine("        }");
                    var bk = otherModel.HasBusinessKey();
                    if (bk != null)
                    {
                        sb.AppendLine($"        function {model.Singular.LocalVariable}{bk.Name.Singular.Value}Template(data) {{");
                        sb.AppendLine($"            return actionLink('@Url.Action(\"GotoDefaultView\", \"{model.Plural.Value}\")', data.Id, data.{bk.Name.Singular.Value});");
                        sb.AppendLine("        }");
                    }
                }
            }
            sb.AppendLine("    </script>");
            sb.AppendLine("}");
            return new File { Name = "Detail.cshtml", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}