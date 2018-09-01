using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebViews
{
    internal class Grid : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;
        private readonly int _indent;
        private readonly string _gridName;
        private readonly List<string> _filters;

        public string DataBound { get; set; }
        public string Change { get; set; }
        public bool Selectable { get; set; } = true;

        public Grid(ISettings settings, Module module, Model model, int indent = 2, string gridName = null, List<string> filters = null)
        {
            if (string.IsNullOrWhiteSpace(gridName))
                gridName = "kendoListGrid";
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _indent = indent;
            _gridName = gridName;
            _filters = filters;
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var i1 = h.Indent(_indent);
            var i2 = h.Indent(_indent + 1);
            var i3 = h.Indent(_indent + 2);
            var i4 = h.Indent(_indent + 3);

            var sb = new StringBuilder();
            sb.AppendLine($"{i1}<kendo-grid name=\"{_gridName}\" navigatable=\"true\" class=\"table-responsive\" style=\"max-height: 500px\"");
            if (!string.IsNullOrEmpty(DataBound) || !string.IsNullOrEmpty(Change) || Selectable)
            {
                sb.Append($"{i1}           ");
                if (!string.IsNullOrEmpty(DataBound))
                    sb.Append($" on-data-bound=\"{DataBound}\"");
                if (!string.IsNullOrEmpty(Change))
                    sb.Append($" on-change=\"{Change}\"");
                if (Selectable)
                    sb.Append($" selectable=\"true\"");
                sb.AppendLine(">");
            }
            sb.AppendLine($"{i2}<columns>");
            var bk = _model.HasBusinessKey();
            if (bk != null)
            {
                var title = "";
                if (bk.Name.Singular.Display != bk.Name.Singular.Value)
                    title = $"title=\"{bk.Name.Singular.Display}\" ";
                sb.AppendLine($"{i3}<column field=\"{bk.Name.Singular.Value}\" {bk.Name.Singular.LocalVariable}Template=\"#={_model.Name.Singular.LocalVariable}{bk.Name.Singular.Value}Template(data)#\" />");
            }
            foreach (var field in _model.Fields)
            {
                if (field == bk)
                {
                    continue;
                }

                var added = false;
                if (field.DataType == DataTypes.Bool)
                {
                    if (field.Name.Singular.Value == "IsActive")
                        sb.AppendLine($"{i3}<column field=\"{field.Name.Singular.Value}\" title=\"{field.Name.Singular.Display}\" template=\"@Constants.IsActiveTemplate\" width=\"100\" />");
                    else
                        sb.AppendLine($"{i3}<column field=\"{field.Name.Singular.Value}\" title=\"{field.Name.Singular.Display}\" template=\"#if (data.{field.Name.Singular.Value}) {{ # <i class='fa fa-check' aria-hidden='true'></i> # }} #\" width=\"100\" />");
                    added = true;
                }
                else if (field.DataType == DataTypes.UniqueIdentifier)
                {
                    foreach (var item in _model.Relationships)
                    {
                        item.GetMatch(_model.Name, out var matchType, out var matchField);
                        item.GetOther(_model.Name, out var otherType, out var otherModel, out var otherField);
                        if (!field.Name.Equals(matchField))
                            continue;

                        var om = _module.Models.Single(m => m.Name.Equals(otherModel));
                        var obk = om.HasBusinessKey();
                        if (obk != null)
                        {
                            sb.AppendLine($"{i3}<column field=\"{om.Name.Singular.Value}{obk.Name.Singular.Value}\" title=\"{om.Name.Singular.Value}\" />");
                            added = true;
                            break;
                        }
                    }
                }
                if (!added)
                {
                    var title = "";
                    if (field.Name.Singular.Display != field.Name.Singular.Value)
                        title = $"title=\"{field.Name.Singular.Display}\" ";
                    sb.AppendLine($"{i3}<column field=\"{field.Name.Singular.Value}\" {title}/>");
                }
            }
            sb.AppendLine($"{i2}</columns>");
            sb.AppendLine($"{i2}<scrollable virtual=\"true\" />");
            var unsort = bk != null ? "false" : "true";
            sb.AppendLine($"{i2}<sortable enabled=\"true\" allow-unsort=\"{unsort}\" mode=\"single\" initial-direction=\"asc\" />");
            sb.AppendLine($"{i2}<filterable enabled=\"true\" />");
            sb.AppendLine($"{i2}<datasource type=\"DataSourceTagHelperType.WebApi\" page-size=\"20\">");
            if (bk != null)
            {
                sb.AppendLine($"{i3}<sorts>");
                sb.AppendLine($"{i4}<sort field=\"{bk.Name.Singular.Value}\" direction=\"asc\" />");
                sb.AppendLine($"{i3}</sorts>");
            }
            if (_model.HasActive() || _filters != null)
            {
                sb.AppendLine($"{i3}<filters>");
                if (_model.HasActive())
                    sb.AppendLine($"{i4}<datasource-filter field=\"IsActive\" operator=\"eq\" value=\"true\" />");
                if (_filters != null)
                {
                    foreach (var filter in _filters)
                    {
                        sb.Append(i4);
                        sb.AppendLine(filter);
                    }
                }
                sb.AppendLine($"{i3}</filters>");
            }
            sb.AppendLine($"{i3}<transport>");
            sb.AppendLine($"{i4}<read type=\"get\" url=\"@Model.{_model.Name.Singular.Value}Uri\" />");
            sb.AppendLine($"{i3}</transport>");

            sb.AppendLine($"{i3}<schema>");
            sb.AppendLine($"{i4}<model>");
            sb.AppendLine($"{i4}    <fields>");

            foreach (var field in _model.Fields)
            {
                sb.AppendLine($"{i4}        <field name=\"{field.Name.Singular.Value}\" type=\"{GridDataType(field)}\"></field>");
            }

            sb.AppendLine($"{i4}    </fields>");
            sb.AppendLine($"{i4}</model>");
            sb.AppendLine($"{i3}</schema>");
            sb.AppendLine($"{i2}</datasource>");
            sb.AppendLine($"{i1}</kendo-grid>");

            return new Snippet(sb.ToString());
        }

        private string GridDataType(Field field)
        {
            switch (field.DataType)
            {
                case DataTypes.Bool:
                    return "boolean";
                case DataTypes.String:
                    return "string";
                case DataTypes.Date:
                case DataTypes.DateTime:
                case DataTypes.Time:
                case DataTypes.DateTimeOffset:
                    return "date";
                case DataTypes.Number:
                    return "number";
                case DataTypes.Object:
                    return "object";
                case DataTypes.UniqueIdentifier:
                default:
                    return "string";
            }
        }
    }
}