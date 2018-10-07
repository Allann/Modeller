using System;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace BusinessService
{
    internal class EditService : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public EditService(ISettings settings, Module module, Model model)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            if (!Settings.SupportRegen || !_model.IsEntity())
                return null;

            var i1 = h.Indent(1);
            var i2 = h.Indent(2);
            var i3 = h.Indent(3);

            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine($"using {_module.Namespace}.Data;");
            sb.AppendLine("using Jbssa.Core.Interfaces;");
            sb.AppendLine("using Jbssa.Core.BusinessLogic;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Business.Services");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}public partial class {_model.Name.Singular.Value}EditService : EditableServiceAsyncBase<Domain.{_model.Name.Singular.Value}>");
            sb.AppendLine($"{i1}{{");
            sb.AppendLine($"{i2}public {_model.Name.Singular.Value}EditService({_module.Project.Singular.Value}DbContext context, IReadableAsync<Domain.{_model.Name.Singular.Value}> readService)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}_context = context ?? throw new ArgumentNullException(nameof(context));");
            sb.AppendLine($"{i3}_readService = readService ?? throw new ArgumentNullException(nameof(readService));");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File { Name = $"{_model.Name.Singular.Value}EditService.cs", Content = sb.ToString(), CanOverwrite = false };
        }
    }
}