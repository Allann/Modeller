using System;
using System.Linq;
using System.Text;
using Modeller;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebViewModels
{
    internal class EditViewModel : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public EditViewModel(ISettings settings, Module module, Model model)
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
            sb.AppendLine($"using {_module.Namespace}.Security;");
            sb.AppendLine("using Jbssa.Core.Mvc.ViewModels;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Web.ViewModels.{_model.Name.Plural.Value}");
            sb.AppendLine("{");
            sb.Append($"    public class {_model.Name.Singular.Value}EditViewModel : ViewModel");
            if (_model.HasAudit)
            {
                sb.AppendLine("Auditable");
            }
            else
            {
                sb.AppendLine("Base");
            }
            sb.AppendLine("    {");
            sb.AppendLine($"        public PageViewModel Page {{ get; }} = new PageViewModel(\"{_model.Name.Singular.Display}\", SupportedResources.{_model.Name.Singular.Value});");
            foreach (var field in _model.Fields)
            {
                var snippet = (ISnippet)new Property.Generator(field, 2) { GuidNullable = true }.Create();
                if (snippet != null)
                {
                    sb.AppendLine();
                    sb.Append(snippet.Content);
                }
                if (field.DataType == DataTypes.UniqueIdentifier)
                {
                    foreach (var item in _model.Relationships)
                    {
                        item.GetMatch(_model.Name, out var matchType, out var matchField);
                        item.GetOther(_model.Name, out var otherType, out var otherModel, out var otherField);
                        if (!field.Name.Equals(matchField))
                            continue;

                        if (matchType == RelationShipTypes.Many)
                        {
                            var om = _module.Models.Single(m => m.Name.Equals(otherModel));
                            var bk = om.HasBusinessKey();
                            if (bk != null)
                            {
                                sb.AppendLine();
                                sb.AppendLine($"        public string {om.Name.Singular.Value}{bk.Name.Singular.Value} {{ get; set; }}");
                                break;
                            }
                        }
                    }
                }
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return new File { Name = _model.Name.Singular.Value + "EditViewModel.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}