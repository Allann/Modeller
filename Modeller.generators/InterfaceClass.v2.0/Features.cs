﻿using System;
using System.Text;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace InterfaceClass
{
    internal class Features : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public Features(ISettings settings, Module module, Model model)
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
            sb.AppendLine($"using {_module.Namespace}.Dto;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Interface");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}public interface I{_model.Name.Singular.Value}Features ");
            sb.AppendLine($"{i1}{{");
            sb.AppendLine($"{i2}// todo: Add custom feature requests here, or delete the file.");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File { Name = $"I{_model.Name.Singular.Value}Features.cs", Content = sb.ToString(), CanOverwrite = false };
        }
    }
}