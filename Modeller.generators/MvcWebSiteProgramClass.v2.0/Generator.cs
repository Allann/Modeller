using System;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebSiteProgram
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

            var sb = new StringBuilder();
            sb.AppendLine("using System.Linq;");
            sb.AppendLine($"using {_module.Namespace}.Data;");
            sb.AppendLine("using Jbssa.Core.BusinessLogic;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Business.Services");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}public class {_model.Name.Singular.Value}ListService : listProviderBase<Domain.{_model.Name.Singular.Value}>");
            sb.AppendLine($"{i1}{{");

            sb.AppendLine($"{i2}public ApplicationListProvider({_module.Project.Singular.Value}DbContext context)");
            sb.AppendLine($"{i3}: base(() => context.Applications)");
            sb.AppendLine($"{i2}{{ }}");
            var bk = _model.HasBusinessKey();
            if (_model.HasActive() || bk != null)
            {
                sb.Append($"{i2}public override IQueryable<Domain.{_model.Name.Singular.Value}> Query => base.Query");
                if (_model.HasActive())
                    sb.Append(".Where(t => t.IsActive)");
                if (bk != null)
                    sb.Append(".OrderBy(t => t.Name)");
                sb.AppendLine(";");
            }
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new Snippet(sb.ToString());
        }

        public ISettings Settings { get; }
    }
}