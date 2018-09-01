using System;
using System.Text;
using Modeller;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebViewModels
{
    internal class FilterViewModel : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public FilterViewModel(ISettings settings, Module module, Model model)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            if (!_model.IsEntity())
                return null;

            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations;");
            sb.AppendLine($"using {_module.Namespace}.Dto;");
            sb.AppendLine($"using {_module.Namespace}.Security;");
            sb.AppendLine("using Jbssa.Core.Mvc.ViewModels;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Web.ViewModels.{_model.Name.Plural.Value}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {_model.Name.Singular.Value}FilterViewModel : BaseQuickFilterViewModel<{_model.Name.Singular.Value}Response>");
            sb.AppendLine("    {");
            sb.AppendLine($"        public {_model.Name.Singular.Value}FilterViewModel()");
            sb.AppendLine("        {");
            sb.AppendLine("            ShowCheckbox = true;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine($"        public PageViewModel Page {{ get; }} = new PageViewModel(\"{_model.Name.Singular.Display}\", SupportedResources.{_model.Name.Singular.Value});");
            foreach (var field in _model.Fields)
            {
                var snippet = (ISnippet)new Property.Generator(field, 2) { GuidNullable = true }.Create();
                if (snippet != null)
                {
                    sb.AppendLine();
                    sb.Append(snippet.Content);
                }
            }

            foreach (var relationship in _model.Relationships)
            {
                relationship.GetOther(_model.Name, out var type, out var model, out var field);
                if (type == RelationShipTypes.One)
                {
                    sb.AppendLine();
                    sb.AppendLine($"        public {model.Singular.Value}Response {model.Singular.Value} {{ get; set; }}");
                }
            }

            sb.AppendLine();
            sb.AppendLine($"        public Flurl.Url {_model.Name.Singular.Value}Uri {{ get; internal set; }}");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return new File { Name = _model.Name.Singular.Value + "FilterViewModel.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}