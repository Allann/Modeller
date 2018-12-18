using System;
using System.Text;
using Hy.Modeller.Interfaces;
using Hy.Modeller.Models;
using Hy.Modeller.Outputs;

namespace ClassLibrary
{
    static class StringBuildExtensions
    {
        internal static StringBuilder Indent(this StringBuilder sb, int indent = 1)
        {
            sb.Append(new string(' ', indent * 4));
            return sb;
        }
    }

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
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}");
            sb.AppendLine("{");
            sb.Indent(1).AppendLine($"public class {_model.Name}Response");
            sb.Indent(1).AppendLine("{");

            foreach (var item in _model.Key.Fields)
            {
                sb.Indent(2).Append($"public {item.DataType} {item.Name} {{ get; set; }}");
                sb.AppendLine();
            }

            foreach (var item in _model.Fields)
            {
                sb.Indent(2).Append($"public {item.DataType} {item.Name} {{ get; set; }}");
                sb.AppendLine();
            }


            sb.Indent(1).AppendLine("}");
            sb.AppendLine("}");

            var file = new File { Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
            var filename = _model.Name.ToString();
            if (Settings.SupportRegen)
            {
                filename += ".generated";
            }
            filename += ".cs";
            file.Name = filename;
            return file;
        }
    }
}