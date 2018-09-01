using System;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace DomainClass
{
    public class DomainUser : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public DomainUser(ISettings settings, Module module, Model model)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            if (!Settings.SupportRegen)
                return null;

            var i1 = h.Indent(1);
            var i2 = h.Indent(2);
            var i3 = h.Indent(3);
            var i4 = h.Indent(4);

            var sb = new StringBuilder();
            sb.AppendLine("using Jbssa.Core;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Domain");
            sb.AppendLine("{");
            sb.Append($"{i1}public partial class {_model.Name.Singular.Value}");
            var entity = string.Empty;
            if (_model.IsEntity())
            {
                entity += " : Entity";
                if (_model.HasAudit)
                {
                    entity += "Auditable";
                }
            }
            sb.AppendLine(entity);

            sb.AppendLine($"{i1}{{");
            sb.AppendLine($"{i2}// todo: add your code here.  Don't change the other partial classes as they will be overwritten.");

            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File { Name = _model.Name.Singular.Value + ".cs", Content = sb.ToString() };
        }
    }
}