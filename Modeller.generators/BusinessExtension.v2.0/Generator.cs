using System;
using System.Linq;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace BusinessExtension
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

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var i1 = h.Indent(1);
            var i2 = h.Indent(2);
            var i3 = h.Indent(3);
            var i4 = h.Indent(4);

            var sb = new StringBuilder();
            sb.AppendLine($"using {_module.Namespace}.Dto;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Business.Extensions");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}public static class {_model.Name.Singular.Value}Extensions");
            sb.AppendLine($"{i1}{{");

            sb.AppendLine($"{i2}public static void UpdateUsing(this Domain.{_model.Name.Singular.Value} left, Domain.{_model.Name.Singular.Value} right)");
            sb.AppendLine($"{i2}{{");
            foreach (var field in _model.Fields)
            {
                sb.AppendLine($"{i3}left.{field.Name.Singular.Value} = right.{field.Name.Singular.Value};");
            }
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}public static {_model.Name.Singular.Value}ListResponse ToListResponse(this Domain.{_model.Name.Singular.Value} s)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}return new {_model.Name.Singular.Value}ListResponse");
            sb.AppendLine($"{i3}{{");
            foreach (var field in _model.Key.Fields)
            {
                sb.AppendLine($"{i4}{field.Name.Singular.Value} = s.{field.Name.Singular.Value},");
            }
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
                                sb.Append($"{i4}{om.Name.Singular.Value}{bk.Name.Singular.Value} = s.{om.Name.Singular.Value}?.{bk.Name.Singular.Value}");
                                added = true;
                                break;
                            }
                        }
                    }
                }
                if (!added)
                    sb.Append($"{i4}{field.Name.Singular.Value} = s.{field.Name.Singular.Value}");
                if (field != _model.Fields.Last())
                    sb.AppendLine(",");
                else
                    sb.AppendLine();
            }
            sb.AppendLine($"{i3}}};");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File { Name = $"{_model.Name.Singular.Value}Extensions.cs", Content = sb.ToString() };
        }
    }
}