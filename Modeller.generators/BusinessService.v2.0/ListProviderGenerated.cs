using System;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace BusinessService
{
    internal class ListProviderGenerated : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public ListProviderGenerated(ISettings settings, Module module, Model model)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            if (!_model.IsEntity())
                return null;

            var i1 = h.Indent(1);
            var i2 = h.Indent(2);
            var i3 = h.Indent(3);
            var i4 = h.Indent(4);
            var i5 = h.Indent(5);

            var sb = new StringBuilder();
            sb.AppendLine(((ISnippet)new Header.Generator(Settings, new GeneratorDetails()).Create()).Content);
            sb.AppendLine("using System.Linq;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Business.Services");
            sb.AppendLine("{");
            sb.Append($"{i1}");
            if (!Settings.SupportRegen)
                sb.Append("public ");
            else
                sb.Append("partial ");
            sb.Append($"class {_model.Name.Singular.Value}ListProvider");
            if (!Settings.SupportRegen)
                sb.Append($" : ListProviderBase<Domain.{_model.Name.Singular.Value}>");
            sb.AppendLine();
            sb.AppendLine($"{i1}{{");
            if (!Settings.SupportRegen)
            {
                sb.AppendLine($"{i2}public {_model.Name.Singular.Value}ListProvider({_module.Project.Singular.Value}DbContext context)");
                sb.AppendLine($"{i3}: base(() => context.{_model.Name.Plural.Value})");
                sb.AppendLine($"{i2}{{ }}");
            }
            else
                sb.AppendLine($"{i2}partial void GetQuery(ref IQueryable<Domain.{_model.Name.Singular.Value}> query);");

            if (Settings.SupportRegen)
            {
                sb.AppendLine();
                sb.AppendLine($"{i2}public override IQueryable<Domain.{_model.Name.Singular.Value}> Query");
                sb.AppendLine($"{i2}{{");
                sb.AppendLine($"{i3}get");
                sb.AppendLine($"{i3}{{");
                sb.AppendLine($"{i4}IQueryable<Domain.{_model.Name.Singular.Value}> query = base.Query;");
                sb.AppendLine($"{i4}GetQuery(ref query);");
                sb.AppendLine($"{i4}return query;");
                sb.AppendLine($"{i3}}}");
                sb.AppendLine($"{i2}}}");
            }
            else
            {
                var bk = _model.HasBusinessKey();
                if (_model.HasActive() || bk != null)
                {
                    sb.Append($"{i2}public override IQueryable<Domain.{_model.Name.Singular.Value}> Query => base.Query");
                    if (_model.HasActive())
                        sb.Append(".Where(t => t.IsActive)");
                    if (bk != null)
                        sb.Append($".OrderBy(t => t.{bk.Name.Singular.Value})");
                    sb.AppendLine(";");
                }
            }
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");
            var name = $"{_model.Name.Singular.Value}ListProvider";
            if (Settings.SupportRegen)
                name += ".generated";
            return new File { Name = $"{name}.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}