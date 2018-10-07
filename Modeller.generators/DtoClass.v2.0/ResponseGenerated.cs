using System;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace DtoClass
{
    internal class ResponseGenerated : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public ResponseGenerated(ISettings settings, Module module, Model model)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var isEntity = _model.IsEntity();

            var sb = new StringBuilder();
            sb.AppendLine(((ISnippet)new Header.Generator(Settings, new GeneratorDetails()).Create()).Content);
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Dto");
            sb.AppendLine("{");

            var i1 = h.Indent(1);
            var i2 = h.Indent(2);
            var i3 = h.Indent(3);

            if (Settings.SupportRegen)
                sb.AppendLine($"{i1}partial class {_model.Name.Singular.Value}Response");
            else
            {
                sb.Append($"{i1}public class {_model.Name.Singular.Value}Response");
                var entity = string.Empty;
                if (isEntity)
                {
                    entity += " : Core.Entity";
                    if (_model.HasAudit)
                    {
                        entity += "Auditable";
                    }
                }
                sb.AppendLine(entity);
            }
            sb.AppendLine($"{i1}{{");
            foreach (var item in _model.Key.Fields)
            {
                if (isEntity && item.Name.Singular.Value == "Id")
                {
                    continue;
                }
                sb.AppendLine();
                var property = (ISnippet)new Property.Generator(item, indent: 2, useDataAnnotations: false).Create();
                sb.Append(property.Content);
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

                        if (otherType == RelationShipTypes.One)
                        {
                            sb.AppendLine();
                            var property = (ISnippet)new Property.Generator(field, indent: 2, useDataAnnotations: false).Create();
                            sb.Append(property.Content);
                            sb.AppendLine();
                            sb.AppendLine($"{i2}public {otherModel.Singular.Value}Response {otherModel.Singular.Value} {{ get; set; }}");
                            added = true;
                            break;
                        }
                    }
                }
                if (!added)
                {
                    sb.AppendLine();
                    var property = (ISnippet)new Property.Generator(field, indent: 2, useDataAnnotations: false).Create();
                    sb.Append(property.Content);
                }
            }
            foreach (var item in _model.Relationships)
            {
                item.GetOther(_model.Name, out var type, out var model, out var field);
                if (type == RelationShipTypes.Many)
                {
                    sb.AppendLine();
                    sb.AppendLine($"{i2}public IReadOnlyCollection<{model.Singular.Value}Response> {model.Plural.Value} {{ get; set; }}");
                }
            }
            var bk = _model.HasBusinessKey();
            if (bk != null && isEntity)
            {
                sb.AppendLine();
                sb.AppendLine($"{i2}public override string BusinessKey => {bk.Name.Singular.Value};");
                sb.AppendLine();
                sb.AppendLine($"{i2}public override string ToString()");
                sb.AppendLine($"{i2}{{");
                sb.AppendLine($"{i3}return BusinessKey;");
                sb.AppendLine($"{i2}}}");
            }
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            var file = new File { Content = sb.ToString(), CanOverwrite=Settings.SupportRegen };
            var filename = $"{_model.Name.Singular.Value}Response";
            if (Settings.SupportRegen)
            {
                filename += ".generated";
            }
            filename += ".cs";
            file.Name = filename;
            return file;
        }
    }
}