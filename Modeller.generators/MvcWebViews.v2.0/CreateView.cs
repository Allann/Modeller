using System;
using System.Linq;
using System.Text;
using Modeller;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebViews
{
    internal class CreateView : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public CreateView(ISettings settings, Module module, Model model)
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
            sb.AppendLine($"@model {_model.Name.Singular.Value}CreateViewModel");
            sb.AppendLine();
            sb.AppendLine("@{");
            sb.AppendLine("    ViewBag.Title = Model.Page.Title;");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("<div class=\"container-fluid\">");
            sb.AppendLine("    <form asp-action=\"Create\" id=\"formDetail\">");
            sb.AppendLine("        <partial name=\"_CreateHeader\" for=\"Page\" />");
            sb.AppendLine();
            sb.AppendLine("        <div class=\"form-horizontal\">");
            sb.AppendLine("            <div asp-validation-summary=\"ModelOnly\" class=\"text-validation\" />");
            sb.AppendLine();
            foreach (var field in _model.Fields)
            {
                sb.Append("            <div class=\"form-group");
                if (!field.Nullable && field.DataType != DataTypes.Bool)
                {
                    sb.Append(" required");
                }

                sb.AppendLine("\">");
                sb.AppendLine($"                <label asp-for=\"{field.Name.Singular.Value}\" class=\"col-md-2 control-label\"></label>");
                sb.AppendLine("                <div class=\"col-md-10\">");
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
                                var om = _module.Models.SingleOrDefault(m => m.Name.Equals(otherModel));
                                if (om == null || !om.IsEntity())
                                    continue;

                                sb.AppendLine($"                    <kendo-combobox for=\"{field.Name.Singular.Value}\" datatextfield=\"Text\" datavaluefield=\"Value\" min-length=\"3\" filter=\"FilterType.Contains\">");
                                sb.AppendLine("                        <datasource type=\"DataSourceTagHelperType.Ajax\" page-size=\"40\">");
                                if (om.HasActive())
                                {
                                    sb.AppendLine("                            <filters>");
                                    sb.AppendLine("                                <datasource-filter field=\"IsActive\" operator=\"eq\" value=\"true\" />");
                                    sb.AppendLine("                            </filters>");
                                }
                                sb.AppendLine("                            <transport>");
                                sb.AppendLine($"                                <read url=\"@Url.RouteUrl(RouteNames.{otherModel.Singular.Value}Lookup)\" />");
                                sb.AppendLine("                            </transport>");
                                sb.AppendLine("                        </datasource>");
                                sb.AppendLine("                    </kendo-combobox>");
                                break;
                            }
                        }

                        break;

                    case DataTypes.Number:
                        if (field.Decimals.HasValue && field.Decimals.Value > 0)
                            sb.AppendLine($"                    <kendo-numerictextbox for=\"{field.Name.Singular.Value}\" spinners=\"false\" restrict-decimals=\"true\" decimals=\"{field.Decimals}\" format=\"@Constants.DecimalFormatVm\" />");
                        else
                            sb.AppendLine($"                    <kendo-numerictextbox for=\"{field.Name.Singular.Value}\" spinners=\"false\" restrict-decimals=\"true\" decimals=\"0\" format=\"@Constants.IntegerFormatVm\" />");

                        break;

                    case DataTypes.Bool:
                        if (field.Name.Singular.Value == "IsActive")
                        {
                            sb.AppendLine($"                    <input id=\"{field.Name.Singular.LocalVariable}\" asp-for=\"{field.Name.Singular.Value}\" type=\"checkbox\" data-on=\"Yes\" data-off=\"No\" data-offstyle=\"warning\" data-onstyle=\"default\" data-toggle=\"toggle\" />");
                        }
                        else
                        {
                            sb.AppendLine($"                    <input id=\"{field.Name.Singular.LocalVariable}\" asp-for=\"{field.Name.Singular.Value}\" type=\"checkbox\" data-onstyle=\"default\" data-toggle=\"toggle\" />");
                        }

                        break;

                    case DataTypes.Date:
                    default:
                        sb.AppendLine($"                    <input asp-for=\"{field.Name.Singular.Value}\" class=\"form-control\" autocomplete=\"off\" />");
                        break;
                }
                sb.AppendLine($"                    <span asp-validation-for=\"{field.Name.Singular.Value}\" class=\"text-validation\"></span>");
                sb.AppendLine("                </div>");
                sb.AppendLine("            </div>");
            }

            sb.AppendLine("        </div>");
            sb.AppendLine("    </form>");
            sb.AppendLine("</div>");
            sb.AppendLine("<partial name=\"_Footer\" for=\"Page.Footer\" />");
            sb.AppendLine();
            sb.AppendLine("@section Scripts {");
            sb.AppendLine("    <partial name=\"_ValidationScriptsPartial\" />");
            sb.AppendLine("    <script type=\"text/javascript\">");
            if (_model.Fields.Any(f => f.DataType == DataTypes.Bool))
            {
                sb.AppendLine("        $(function() {");
                foreach (var field in _model.Fields.Where(f => f.DataType == DataTypes.Bool))
                {
                    sb.AppendLine($"            $('#{field.Name.Singular.LocalVariable}').bootstrapToggle();");
                }
                sb.AppendLine("        });");
            }

            sb.AppendLine("        $(document).ready(function () {");
            sb.AppendLine("            setDetailFormAsNotDirty();");
            sb.AppendLine("            setFocus(\"#formDetail\");");
            sb.AppendLine("        })");
            sb.AppendLine("    </script>");
            sb.AppendLine("}");
            return new File { Name = "Create.cshtml", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}