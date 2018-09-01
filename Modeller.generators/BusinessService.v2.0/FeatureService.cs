using System;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace BusinessClass
{
    internal class FeatureService : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public FeatureService(ISettings settings, Module module, Model model)
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

            var sb = new StringBuilder();
            sb.AppendLine($"using {_module.Namespace}.Dto;");
            sb.AppendLine($"using {_module.Namespace}.Interface;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Business.Services");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}public class {_model.Name.Singular.Value}Features : I{_model.Name.Singular.Value}Features");
            sb.AppendLine($"{i1}{{");
            sb.AppendLine($"{i2}private readonly Data.{_module.Project.Singular.Value}DbContext _context;");
            sb.AppendLine();
            sb.AppendLine($"{i2}public {_model.Name.Singular.Value}Features({_module.Project.Singular.Value}.Data.{_module.Project.Singular.Value}DbContext context)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}_context = context ?? throw new ArgumentNullException(nameof(context));");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File { Name = $"{_model.Name.Singular.Value}Features.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}