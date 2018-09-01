using System;
using System.Text;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace ApiMapping
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

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var i1 = h.Indent(1);
            var i2 = h.Indent(2);
            var i3 = h.Indent(3);

            var sb = new StringBuilder();
            sb.AppendLine("using AutoMapper;");
            sb.AppendLine($"using {_module.Namespace}.Dto;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Api.Mappings");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}public class {_model.Name.Singular.Value}Mapping : Profile");
            sb.AppendLine($"{i1}{{");

            sb.AppendLine($"{i2}public {_model.Name.Singular.Value}Mapping()");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}CreateMap<Domain.{_model.Name.Singular.Value}, {_model.Name.Singular.Value}Response>();");
            sb.AppendLine($"{i3}CreateMap<{_model.Name.Singular.Value}CreateRequest, Domain.{_model.Name.Singular.Value}>();");
            sb.AppendLine($"{i3}CreateMap<{_model.Name.Singular.Value}UpdateRequest, Domain.{_model.Name.Singular.Value}>().ReverseMap();");
            sb.AppendLine($"{i2}}}");

            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File { Name = $"{_model.Name.Singular.Value}Mapping.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}