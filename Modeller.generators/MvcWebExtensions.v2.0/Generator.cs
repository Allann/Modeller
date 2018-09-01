using System;
using System.Linq;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebExtensions
{
    public class Generator : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public Generator(ISettings settings, Module module, Model model)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public IOutput Create()
        {
            var i1 = h.Indent(1);
            var i2 = h.Indent(2);
            var i3 = h.Indent(3);
            var i4 = h.Indent(4);

            var sb = new StringBuilder();

            sb.AppendLine("using Kendo.Mvc.UI.Fluent;");
            sb.AppendLine("using Jbssa.Core.Mvc.ViewModels;");
            sb.AppendLine($"using {_module.Namespace}.Dto;");
            sb.AppendLine($"using {_module.Namespace}.Web.ViewModels.{_model.Name.Plural.Value};");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Web.Extensions");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}public static class {_model.Name.Singular.Value}ViewModelExtensions");
            sb.AppendLine($"{i1}{{");
            //sb.AppendLine($"        public static {_model.Name.Singular.Value}ValueUpdateRequest CreateRequest(this {_model.Name.Singular.Value}ValueViewModel model)");
            //sb.AppendLine("        {");
            //sb.AppendLine($"            var request = new {_model.Name.Singular.Value}ValueUpdateRequest");
            //sb.AppendLine("            {");
            //sb.AppendLine($"                Id = model.{_model.Name.Singular.Value}Id.GetValueOrDefault(),");
            //foreach (var field in _model.Fields)
            //{
            //    sb.Append($"                {field.Name.Singular.Value} = model.{field.Name.Singular.Value}");
            //    if (field != _model.Fields.Last())
            //    {
            //        sb.AppendLine(",");
            //    }
            //    else
            //        sb.AppendLine();
            //}
            //sb.AppendLine("            };");
            //sb.AppendLine("            return request;");
            //sb.AppendLine("        }");
            //sb.AppendLine();
            sb.AppendLine($"{i2}public static {_model.Name.Singular.Value}CreateViewModel GetCreateViewModel(this {_model.Name.Singular.Value}Response model)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}var viewModel = new {_model.Name.Singular.Value}CreateViewModel");
            sb.AppendLine($"{i3}{{");
            foreach (var field in _model.Fields)
            {
                var added = false;
                if (field.DataType == DataTypes.UniqueIdentifier)
                {
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
                                sb.AppendLine($"{i4}{field.Name.Singular.Value} = model.{otherModel.Singular.Value}?.{otherField.Singular.Value},");
                                sb.Append($"{i4}{otherModel.Singular.Value}{bk.Name.Singular.Value} = model.{otherModel.Singular.Value}?.{bk.Name.Singular.Value}");
                                added = true;
                                break;
                            }
                        }
                    }
                }
                if (!added)
                    sb.Append($"{i4}{field.Name.Singular.Value} = model.{field.Name.Singular.Value}");

                if (field != _model.Fields.Last())
                    sb.AppendLine(",");
                else
                    sb.AppendLine();
            }
            sb.AppendLine($"{i3}}};");
            sb.AppendLine($"{i3}return viewModel;");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}public static {_model.Name.Singular.Value}DetailViewModel GetDetailViewModel(this {_model.Name.Singular.Value}Response model, BaseQuickFilterViewModel<{_model.Name.Singular.Value}Response> filter)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}var viewModel = new {_model.Name.Singular.Value}DetailViewModel(");
            foreach (var field in _model.Fields)
            {
                var added = false;
                if (field.DataType == DataTypes.UniqueIdentifier)
                {
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
                                sb.AppendLine($"{i4}model.{otherModel.Singular.Value}.{otherField.Singular.Value},");
                                sb.Append($"{i4}model.{otherModel.Singular.Value}.{bk.Name.Singular.Value}");
                                added = true;
                                break;
                            }
                        }
                    }
                }
                if (!added)
                    sb.Append($"{i4}model.{field.Name.Singular.Value}");
                if (field != _model.Fields.Last())
                    sb.AppendLine(",");
                else
                    sb.AppendLine();
            }
            sb.AppendLine("            )");
            sb.AppendLine("            {");
            sb.AppendLine("                StoredQueryId = filter?.StoredQueryId ?? \"\"");
            sb.AppendLine("            };");
            sb.AppendLine("            viewModel.PopulateAuditAndId(model);");
            sb.AppendLine("            viewModel.Page.BusinessKey = model.Id.ToString();");
            sb.AppendLine("            if (viewModel is ViewModelAuditable audit)");
            sb.AppendLine("                viewModel.Page.Footer.Right = audit.AuditString;");
            sb.AppendLine("            return viewModel;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public static {_model.Name.Singular.Value}EditViewModel GetEditViewModel(this {_model.Name.Singular.Value}Response model, BaseQuickFilterViewModel<{_model.Name.Singular.Value}Response> filter)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var viewModel = new {_model.Name.Singular.Value}EditViewModel");
            sb.AppendLine("            {");
            foreach (var field in _model.Fields)
            {
                var added = false;
                if (field.DataType == DataTypes.UniqueIdentifier)
                {
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
                                sb.AppendLine($"{i4}{field.Name.Singular.Value} = model.{otherModel.Singular.Value}?.{otherField.Singular.Value},");
                                sb.AppendLine($"{i4}{otherModel.Singular.Value}{bk.Name.Singular.Value} = model.{otherModel.Singular.Value}?.{bk.Name.Singular.Value},");
                                added = true;
                                break;
                            }
                        }
                    }
                }
                if (!added)
                    sb.AppendLine($"{i4}{field.Name.Singular.Value} = model.{field.Name.Singular.Value},");
            }
            sb.AppendLine("                StoredQueryId = filter?.StoredQueryId ?? \"\"");
            sb.AppendLine("            };");
            sb.AppendLine("            viewModel.PopulateAuditAndId(model);");
            sb.AppendLine("            viewModel.Page.BusinessKey = model.Id.ToString();");
            sb.AppendLine();
            sb.AppendLine("            if (viewModel is ViewModelAuditable audit)");
            sb.AppendLine("                viewModel.Page.Footer.Right = audit.AuditString;");
            sb.AppendLine("            return viewModel;");
            sb.AppendLine("        }");
            sb.AppendLine();
            //sb.AppendLine($"        public static {_model.Name.Singular.Value}ListViewModel GetListViewModel(this {_model.Name.Singular.Value}ListResponse model)");
            //sb.AppendLine("        {");
            //sb.AppendLine($"            var viewModel = new {_model.Name.Singular.Value}ListViewModel");
            //sb.AppendLine("            {");
            //sb.AppendLine("                Id = model.Id,");
            //foreach (var field in _model.Fields)
            //{
            //    sb.Append($"                {field.Name.Singular.Value} = model.{field.Name.Singular.Value},");
            //    if (field != _model.Fields.Last())
            //    {
            //        sb.AppendLine(",");
            //    }
            //    else
            //        sb.AppendLine();
            //}
            //sb.AppendLine("            };");
            //sb.AppendLine("            return viewModel;");
            //sb.AppendLine("        }");
            //sb.AppendLine();
            sb.AppendLine($"        public static {_model.Name.Singular.Value}CreateRequest CreateModel(this {_model.Name.Singular.Value}CreateViewModel viewModel)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var model = new {_model.Name.Singular.Value}CreateRequest");
            sb.AppendLine("            {");
            foreach (var field in _model.Fields)
            {
                sb.Append($"                {field.Name.Singular.Value} = viewModel.{field.Name.Singular.Value}");
                if (field.DataType == DataTypes.UniqueIdentifier)
                    sb.Append(".GetValueOrDefault()");
                if (field != _model.Fields.Last())
                {
                    sb.AppendLine(",");
                }
                else
                    sb.AppendLine();
            }
            sb.AppendLine("            };");
            sb.AppendLine("            return model;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public static {_model.Name.Singular.Value}UpdateRequest CreateUpdateModel(this {_model.Name.Singular.Value}EditViewModel viewModel)");
            sb.AppendLine("        {");
            sb.AppendLine($"            var model = new {_model.Name.Singular.Value}UpdateRequest");
            sb.AppendLine("            {");
            sb.AppendLine("                Id = viewModel.Id,");
            foreach (var field in _model.Fields)
            {
                sb.Append($"                {field.Name.Singular.Value} = viewModel.{field.Name.Singular.Value}");
                if (field.DataType == DataTypes.UniqueIdentifier)
                    sb.Append(".GetValueOrDefault()");
                if (field != _model.Fields.Last())
                {
                    sb.AppendLine(",");
                }
                else
                    sb.AppendLine();
            }
            sb.AppendLine("            };");
            sb.AppendLine("            return model;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return new File() { Content = sb.ToString(), Name = $"{_model.Name.Singular.Value}ViewModelExtensions.cs" };
        }

        public ISettings Settings { get; }
    }
}