using System;
using System.Linq;
using System.Text;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace DomainClass
{
    public class DomainAction : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public DomainAction(ISettings settings, Module module, Model model)
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

            bool first;
            var sb = new StringBuilder();
            sb.AppendLine(((ISnippet)new Header.Generator(Settings, new GeneratorDetails()).Create()).Content);
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Domain");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}partial class {_model.Name.Singular.Value}");
            sb.AppendLine($"{i1}{{");
            if (Settings.SupportRegen)
            {
                sb.AppendLine($"{i2}static partial void CreateEntity(ref {_model.Name.Singular.Value} {_model.Name.Singular.LocalVariable});");
            }
            sb.AppendLine($"{i2}private {_model.Name.Singular.Value}() {{ }}");
            sb.AppendLine();
            sb.AppendLine($"{i2}public static {_model.Name.Singular.Value} Create()");
            sb.AppendLine($"{i2}{{");
            if (Settings.SupportRegen)
            {
                sb.AppendLine($"{i3}{_model.Name.Singular.Value} {_model.Name.Singular.LocalVariable} = null;");
                sb.AppendLine($"{i3}CreateEntity(ref {_model.Name.Singular.LocalVariable});");
                sb.AppendLine($"{i3}if ({_model.Name.Singular.LocalVariable} == null)");
                sb.AppendLine($"{i3}{{");
                sb.Append($"{i4}{_model.Name.Singular.LocalVariable} = new {_model.Name.Singular.Value} {{ ");
                first = true;
                foreach (var f in _model.Fields.Where(f => !string.IsNullOrEmpty(f.Default)))
                {
                    if (!first)
                    {
                        sb.Append(", ");
                    }
                    if (f.DataType == DataTypes.String)
                    {
                        sb.Append($"{f.Name.Singular.Value} = \"{f.Default}\"");
                    }
                    else
                    {
                        sb.Append($"{f.Name.Singular.Value} = {f.Default}");
                    }
                    first = false;
                }
                sb.AppendLine(" };");
                sb.AppendLine($"{i3}}}");
                sb.AppendLine($"{i3}return {_model.Name.Singular.LocalVariable};");
            }
            else
            {
                sb.Append($"{i3}return new {_model.Name.Singular.Value} {{ ");
                first = true;
                foreach (var f in _model.Fields.Where(f => !string.IsNullOrEmpty(f.Default)))
                {
                    if (!first)
                    {
                        sb.Append(", ");
                    }
                    if (f.DataType == DataTypes.String)
                    {
                        sb.Append($"{f.Name.Singular.Value} = \"{f.Default}\"");
                    }
                    else
                    {
                        sb.Append($"{f.Name.Singular.Value} = {f.Default}");
                    }
                    first = false;
                }
                sb.AppendLine(" };");
            }
            sb.AppendLine($"{i2}}}");

            var fs = _model.Fields.Where(f => !f.Nullable && f.Name.Singular.Value != "IsActive");
            if (fs.Any())
            {
                sb.AppendLine();
                sb.Append($"{i2}public static {_model.Name.Singular.Value} Create(");
                first = true;
                foreach (var field in fs)
                {
                    if (!first)
                    {
                        sb.Append(", ");
                    }

                    sb.Append($"{h.DataType(field)} {field.Name.Singular.LocalVariable}");
                    first = false;
                }
                sb.AppendLine(")");
                sb.AppendLine($"{i2}{{");
                sb.AppendLine($"{i3}var result = Create();");

                foreach (var field in fs)
                {
                    sb.AppendLine($"{i3}result.{field.Name.Singular.Value} = {field.Name.Singular.LocalVariable};");
                }
                sb.AppendLine($"{i3}return result;");
                sb.AppendLine($"{i2}}}");
            }
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File() { Name = _model.Name.Singular.Value + ".actions.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}