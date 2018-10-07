using System;
using System.Text;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace Api
{
    internal class ControllerMapper : IGenerator
    {
        private readonly Module _module;

        public ControllerMapper(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var i1 = h.Indent(1);
            var i2 = h.Indent(2);
            var i3 = h.Indent(3);

            var sb = new StringBuilder();
            sb.AppendLine(((ISnippet)new Header.Generator(Settings, new GeneratorDetails()).Create()).Content);
            sb.AppendLine($"namespace {_module.Namespace}.Interface");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}public interface IHasMapper<TDomain>");
            sb.AppendLine($"{i2}where TDomain : class");
            sb.AppendLine($"{i1}{{");
            sb.AppendLine($"{i2}IControllerMapper<TDomain> GetMapper();");
            sb.AppendLine($"{i1}}}");

            sb.AppendLine($"{i1}public interface IControllerMapper<TDomain>");
            sb.AppendLine($"{i1}{{");
            sb.AppendLine($"{i2}TMap Map<TMap>(object entity) where TMap : class;");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");
            return new File { Name = "IControllerMapper.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}