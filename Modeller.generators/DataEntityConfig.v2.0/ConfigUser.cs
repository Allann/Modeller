using System;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace DataEntityConfig
{
    public class ConfigUser : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public ConfigUser(ISettings settings, Module module, Model model)
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
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Data.EntityMappings");
            sb.AppendLine("{");
            sb.Append($"{i1}public partial class {_model.Name.Singular.Value}Configuration");
            if (_model.IsEntity())
                sb.AppendLine($" : EntityTypeConfiguration<Domain.{_model.Name.Singular.Value}>");
            else
                sb.AppendLine();
            sb.AppendLine($"{i1}{{");
            sb.AppendLine($"{i2}partial void AfterConfigure(EntityTypeBuilder<Domain.{_model.Name.Singular.Value}> builder)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}// todo: add additional configuration here");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File() { Name = $"{_model.Name.Singular.Value}Configuration.cs", Content = sb.ToString(), CanOverwrite = false };
        }
    }
}