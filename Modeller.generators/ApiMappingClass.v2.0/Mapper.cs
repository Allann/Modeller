using System;
using System.Text;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace ApiMapping
{
    internal class Mapper : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public Mapper(ISettings settings, Module module, Model model)
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

            var sb = new StringBuilder();
            sb.AppendLine("using AutoMapper;");
            sb.AppendLine($"using {_module.Namespace}.Interfaces;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Api.Mappings");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}public class {_model.Name.Singular.Value}Mapper : IControllerMapper");
            sb.AppendLine($"{i1}{{");
            sb.AppendLine($"{i2}private readonly IMapper _mapper;");
            sb.AppendLine();
            sb.AppendLine($"{i2}public {_model.Name.Singular.Value}Mapper(IMapper mapper)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}_mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}T IControllerMapper.Map<T>(object entity) => _mapper.Map<T>(entity);");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File { Name = $"{_model.Name.Singular.Value}Mapper.cs", Content = sb.ToString(), CanOverwrite = false };
        }
    }
}