using System;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace DomainClass
{
    public class DomainGenerated : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public DomainGenerated(ISettings settings, Module module, Model model)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public IOutput Create()
        {
            var bk = _model.HasBusinessKey();
            var isEntity = _model.IsEntity();
            var i1 = h.Indent(1);
            var i2 = h.Indent(2);
            var i3 = h.Indent(3);

            var sb = new StringBuilder();
            sb.AppendLine(((ISnippet)new Header.Generator(Settings, new GeneratorDetails()).Create()).Content);
            sb.AppendLine("using Jbssa.Core;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Domain");
            sb.AppendLine("{");
            if (Settings.SupportRegen)
            {
                sb.AppendLine($"{i1}partial class {_model.Name.Singular.Value}");
            }
            else
            {
                sb.Append($"{i1}public partial class {_model.Name.Singular.Value}");
                var entity = string.Empty;
                if (isEntity)
                {
                    entity += " : Entity";
                    if (_model.HasAudit)
                    {
                        entity += "Auditable";
                    }
                }
                sb.AppendLine(entity);
            }
            sb.AppendLine($"{i1}{{");

            if (Settings.SupportRegen && bk != null && isEntity)
            {
                sb.AppendLine($"{i2}partial void GetStringValue(ref string value);");
                sb.AppendLine();
            }

            foreach (var item in _model.Key.Fields)
            {
                if (isEntity && item.Name.Singular.Value == "Id")
                    continue;
                var property = (ISnippet)new Property.Generator(item).Create();
                sb.AppendLine(property.Content);
            }
            foreach (var item in _model.Fields)
            {
                var property = (ISnippet)new Property.Generator(item).Create();
                sb.AppendLine(property.Content);
            }

            if (bk != null && isEntity)
            {
                sb.AppendLine($"{i2}public override string BusinessKey => {bk.Name.Singular.Value};");

                sb.AppendLine($"{i2}public override string ToString()");
                sb.AppendLine($"{i2}{{");
                if (Settings.SupportRegen)
                {
                    sb.AppendLine($"{i3}var value = BusinessKey;");
                    sb.AppendLine($"{i3}GetStringValue(ref value);");
                    sb.AppendLine($"{i3}return value;");
                }
                else
                {
                    sb.AppendLine($"{i3}return BusinessKey;");
                }
                sb.AppendLine($"{i2}}}");
            }
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            var file = new File() { Content = sb.ToString() };
            var filename = _model.Name.Singular.Value;
            if (Settings.SupportRegen)
            {
                file.CanOverwrite = true;
                filename += ".generated";
            }
            filename += ".cs";
            file.Name = filename;
            return file;
        }

        public ISettings Settings { get; }
    }
}