using System;
using System.Text;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace DtoClass
{
    internal class UpdateRequestGenerated : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public UpdateRequestGenerated(ISettings settings, Module module, Model model)
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

            var sb = new StringBuilder();
            sb.AppendLine(((ISnippet)new Header.Generator(Settings, new GeneratorDetails()).Create()).Content);
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Dto");
            sb.AppendLine("{");
            if (Settings.SupportRegen)
                sb.AppendLine($"{i1}partial class {_model.Name.Singular.Value}UpdateRequest");
            else
                sb.AppendLine($"{i1}public class {_model.Name.Singular.Value}UpdateRequest");
            sb.AppendLine($"{i1}{{");
            foreach (var item in _model.Key.Fields)
            {
                sb.AppendLine();
                var property = (ISnippet)new Property.Generator(item, indent: 2, useDataAnnotations: false).Create();
                sb.Append(property.Content);
            }
            foreach (var item in _model.Fields)
            {
                sb.AppendLine();
                var property = (ISnippet)new Property.Generator(item, indent: 2, useDataAnnotations: false).Create();
                sb.Append(property.Content);
            }
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            var file = new File { Content = sb.ToString() };
            var filename = $"{_model.Name.Singular.Value}UpdateRequest";
            if (Settings.SupportRegen)
            {
                file.CanOverwrite = true;
                filename += ".generated";
            }
            filename += ".cs";
            file.Name = filename;
            return file;
        }
    }
}