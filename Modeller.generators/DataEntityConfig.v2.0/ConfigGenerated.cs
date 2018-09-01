using System;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace DataEntityConfig
{
    public class ConfigGenerated: IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public ConfigGenerated(ISettings settings, Module module, Model model)
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

            var sb = new StringBuilder();
            sb.AppendLine(((ISnippet)new Header.Generator(Settings, new GeneratorDetails()).Create()).Content);
            sb.AppendLine("using Jbssa.Core;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore.Metadata.Builders;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Data.EntityMappings");
            sb.AppendLine("{");
            sb.Append($"{i1}");
            if (!Settings.SupportRegen)
                sb.Append("public ");
            sb.Append($"partial class {_model.Name.Singular.Value}Configuration");
            if (_model.IsEntity() && !Settings.SupportRegen)
                sb.AppendLine($" : EntityTypeConfiguration<Domain.{_model.Name.Singular.Value}>");
            else
                sb.AppendLine();
            sb.AppendLine($"{i1}{{");
            sb.AppendLine($"{i2}public override void Configure(EntityTypeBuilder<Domain.{_model.Name.Singular.Value}> builder)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}base.Configure(builder);");
            sb.AppendLine();
            foreach (var field in _model.Fields)
            {
                if (field.DataType == DataTypes.String)
                {
                    sb.AppendLine($"{i3}builder.Property(p => p.{field.Name.Singular.Value})");
                    if (!field.Nullable)
                    {
                        sb.AppendLine($"{i4}.IsRequired()");
                    }
                    if (field.MaxLength.HasValue)
                    {
                        sb.AppendLine($"{i4}.HasMaxLength({field.MaxLength.Value});");
                    }
                }
            }
            if (_model.HasBusinessKey() != null)
            {
                sb.AppendLine();
                sb.AppendLine($"{i3}builder.HasIndex(i => i.{_model.HasBusinessKey().Name.Singular.Value}).IsUnique().ForSqlServerIsClustered();");
            }

            if (Settings.SupportRegen)
            {
                sb.AppendLine();
                sb.AppendLine($"{i3}AfterConfigure(builder);");
                sb.AppendLine($"{i2}}}");
                sb.AppendLine();
                sb.AppendLine($"{i2}partial void AfterConfigure(EntityTypeBuilder<Domain.{_model.Name.Singular.Value}> builder);");
            }
            else
            {
                sb.AppendLine($"{i2}}}");
            }
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            var name = $"{ _model.Name.Singular.Value }Configuration";
            if (Settings.SupportRegen)
                name += ".generated";
            return new File() { Name = $"{name}.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}