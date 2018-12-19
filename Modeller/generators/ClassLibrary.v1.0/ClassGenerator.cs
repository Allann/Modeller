using Hy.Modeller.Interfaces;
using Hy.Modeller.Models;
using Hy.Modeller.Outputs;
using System;
using System.Text;

namespace ClassLibrary
{
    internal static class ClassGenerator 
    {
        internal static IFile Create(Module module, Model model, ISettings settings)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
            sb.AppendLine($"namespace {module.Namespace}");
            sb.AppendLine("{");
            sb.Indent(1).AppendLine($"public class {model.Name}Response");
            sb.Indent(1).AppendLine("{");

            foreach (var item in model.Key.Fields)
            {
                sb.Indent(2).Append($"public {item.DataType} {item.Name} {{ get; set; }}");
                sb.AppendLine();
            }

            foreach (var item in model.Fields)
            {
                sb.Indent(2).Append($"public {item.DataType} {item.Name} {{ get; set; }}");
                sb.AppendLine();
            }


            sb.Indent(1).AppendLine("}");
            sb.AppendLine("}");

            var file = new File { Content = sb.ToString(), CanOverwrite = settings.SupportRegen };
            var filename = model.Name.ToString();
            if (settings.SupportRegen)
            {
                filename += ".generated";
            }
            filename += ".cs";
            file.Name = filename;
            return file;
        }
    }
}
