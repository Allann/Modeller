using System;
using System.Linq;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace DtoClass
{
    internal class ListResponseGenerated : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public ListResponseGenerated(ISettings settings, Module module, Model model)
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

            var sb = new StringBuilder();
            sb.AppendLine(((ISnippet)new Header.Generator(Settings, new GeneratorDetails()).Create()).Content);
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Dto");
            sb.AppendLine("{");
            if (Settings.SupportRegen)
                sb.AppendLine($"{i1}partial class {_model.Name.Singular.Value}ListResponse");
            else
                sb.AppendLine($"{i1}public class {_model.Name.Singular.Value}ListResponse");
            sb.AppendLine($"{i1}{{");
            foreach (var item in _model.Key.Fields)
            {
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

                        if (matchType == RelationShipTypes.Many)
                        {
                            var om = _module.Models.Single(m => m.Name.Equals(otherModel));
                            var bk = om.HasBusinessKey();
                            if (bk != null)
                            {
                                sb.AppendLine();
                                sb.AppendLine($"{i2}public string {om.Name.Singular.Value}{bk.Name.Singular.Value} {{ get; set; }}");
                                added = true;
                                break;
                            }
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
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            var file = new File { Content = sb.ToString() };
            var filename = $"{_model.Name.Singular.Value}ListResponse";
            if (Settings.SupportRegen)
            {
                file.CanOverwrite = true;
                filename += ".generated";
            }
            filename += ".cs";
            file.Name = filename;
            return file;
        }
    }
}