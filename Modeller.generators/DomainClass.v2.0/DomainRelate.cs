using System;
using System.Text;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace DomainClass
{
    public class DomainRelate : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public DomainRelate(ISettings settings, Module module, Model model)
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

            sb.AppendLine("using Jbssa.Core.Extensions;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Domain");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}partial class {_model.Name.Singular.Value}");
            sb.Append($"{i1}{{");

            foreach (var relate in _model.Relationships)
            {
                try
                {
                    relate.GetOther(_model.Name, out var type, out var model, out var field);
                    switch (type)
                    {
                        case RelationShipTypes.Zero:
                        case RelationShipTypes.One:
                            AddSingle(sb, model);
                            break;

                        case RelationShipTypes.Many:
                            AddList(sb, model);
                            break;

                        default:
                            break;
                    }
                }
                catch (ApplicationException ex)
                {
                    var bg = Console.BackgroundColor;
                    var fg = Console.ForegroundColor;
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("{0}.{1} -> {2}.{3}", relate.LeftModel, relate.LeftField, relate.RightModel, relate.RightField);
                    Console.BackgroundColor = bg;
                    Console.ForegroundColor = fg;
                }
            }

            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File() { Name = _model.Name.Singular.Value + ".relate.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }

        private void AddList(StringBuilder sb, Name relate)
        {
            sb.AppendLine();
            sb.AppendLine($"{h.Indent(2)}public List<{relate.Singular.Value}> {relate.Plural.Value} {{ get; set; }} = new List<{relate.Singular.Value}>();");
        }

        private void AddSingle(StringBuilder sb, Name relate)
        {
            sb.AppendLine();
            sb.AppendLine($"{h.Indent(2)}public {relate.Singular.Value} {relate.Singular.Value} {{ get; set; }}");
        }
    }
}