using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;
using System;
using System.Linq;
using System.Text;

namespace WebControllerSidebar
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

        public IOutput Create()
        {
            var i1 = h.Indent(1);
            var i2 = h.Indent(2);
            var i3 = h.Indent(3);
            var i4 = h.Indent(4);

            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("using Jbssa.Core.Mvc.ViewModels;");
            sb.AppendLine($"using {_module.Namespace}.Dto;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine("using Newtonsoft.Json;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Web.Controllers");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}partial class {_model.Name.Plural.Value}Controller");
            sb.AppendLine($"{i1}{{");
            sb.AppendLine($"{i2}partial void AddLinks(SidebarViewModel sidebar, {_model.Name.Singular.Value}Response data)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}Guid? {_model.Name.Singular.Value}Id = data?.Id;");
            foreach (var item in _model.Relationships.Where(r=>r.RightType==RelationShipTypes.Many))
            {
                var name = item.RightModel;
                sb.AppendLine($"{i3}var {name.Singular.LocalVariable}Link = new ActionSidebarItem(\"lnk{name.Singular.Value}\", \"Manage {name.Plural.Display}\", \"{name.Plural.Value}\", \"For{_model.Name.Singular.Value}\")");
                sb.AppendLine($"{i3}{{");
                sb.AppendLine($"{i4}Enabled = {_model.Name.Singular.Value}Id.HasValue");
                sb.AppendLine($"{i3}}};");
                sb.AppendLine($"{i3}{name.Singular.LocalVariable}Link.Parameters.Add(\"{_model.Name.Singular.Value}Id\", {_model.Name.Singular.Value}Id.GetValueOrDefault().ToString());");
                sb.AppendLine($"{i3}sidebar.AddLink(\"default\", {name.Singular.LocalVariable}Link);");
            }
            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}[HttpPost]");
            sb.AppendLine($"{i2}public IActionResult SidebarItems(string selectedData)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}{_model.Name.Singular.Value}Response data = null;");
            sb.AppendLine($"{i3}if (selectedData != null)");
            sb.AppendLine($"{i3}{{");
            sb.AppendLine($"{i4}data = JsonConvert.DeserializeObject<{_model.Name.Singular.Value}Response>(selectedData);");
            sb.AppendLine($"{i3}}}");
            sb.AppendLine($"{i3}var viewModel = new SidebarViewModel();");
            sb.AppendLine($"{i3}AddLinks(viewModel, data);");
            sb.AppendLine($"{i3}return PartialView(\"_Sidebar\", viewModel);");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new Snippet(sb.ToString());
        }

        public ISettings Settings { get; }
    }
}
