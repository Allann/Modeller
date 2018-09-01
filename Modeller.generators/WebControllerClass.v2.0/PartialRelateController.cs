using System;
using System.Linq;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace WebController
{
    internal class PartialRelateController : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;
        
        public PartialRelateController(ISettings settings, Module module, Model model)
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

            var fields = _model.Fields.ToList();
            fields.AddRange(_model.Key.Fields);
            var parentModel = _model.Name;

            var files = new FileGroup();
            foreach (var item in _model.Relationships)
            {
                item.GetMatch(parentModel, out var parentType, out var tempField);
                var parentField = fields.Single(f => f.Name.Equals(tempField));
                item.GetOther(parentModel, out var childType, out var childModel, out var childField);
                if (childType != RelationShipTypes.Many)
                    continue;

                var cm = _module.Models.FirstOrDefault(m => m.Name.Equals(childModel));
                if (cm == null || !cm.IsEntity())
                    continue;

                var sb = new StringBuilder();
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Threading.Tasks;");
                sb.AppendLine("using Flurl;");
                sb.AppendLine("using Flurl.Http;");
                sb.AppendLine($"using {_module.Namespace}.Dto;");
                sb.AppendLine($"using {_module.Namespace}.Web.ViewModels.{childModel.Plural.Value};");
                sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
                sb.AppendLine();
                sb.AppendLine($"namespace {_module.Namespace}.Web.Controllers");
                sb.AppendLine("{");
                sb.AppendLine($"{i1}partial class {childModel.Plural.Value}Controller");
                sb.AppendLine($"{i1}{{");
                sb.AppendLine($"{i2}[Route(\"[controller]/For{parentModel.Singular.Value}\")]");
                sb.AppendLine($"{i2}public async Task<IActionResult> GetFor{parentModel.Singular.Value}([FromQuery] Guid {childField.Singular.LocalVariable})");
                sb.AppendLine($"{i2}{{");
                sb.AppendLine($"{i2}    var url = _baseUri");
                sb.AppendLine($"{i2}        .AppendPathSegments(KnownApiRoutes.{parentModel.Plural.Value}.Name, {childField.Singular.LocalVariable});");
                sb.AppendLine($"{i2}    var model = await url.GetJsonAsync<{parentModel.Singular.Value}Response>();");
                sb.AppendLine($"{i2}    if (model == null)");
                sb.AppendLine($"{i2}        return NotFound();");
                sb.AppendLine();
                sb.AppendLine($"{i2}    var viewModel = new {childModel.Singular.Value}FilterViewModel");
                sb.AppendLine($"{i2}    {{");
                sb.AppendLine($"{i2}        {childField.Singular.Value} = {childField.Singular.LocalVariable},");
                sb.AppendLine($"{i2}        {parentModel.Singular.Value} = model,");
                sb.AppendLine($"{i2}        {childModel.Singular.Value}Uri = _baseUri.AppendPathSegments(KnownApiRoutes.{childModel.Plural.Value}.Name),");
                sb.AppendLine($"{i2}    }};");
                sb.AppendLine($"{i2}    viewModel.Page.Referer = Request.Headers[\"Referer\"].ToString();");
                sb.AppendLine($"{i2}    AddLinks(viewModel.Page.Sidebar, null, model.Id);");
                sb.AppendLine($"{i2}    return View(\"For{parentModel.Singular.Value}\", viewModel);");
                sb.AppendLine($"{i2}}}");
                sb.AppendLine();
                sb.AppendLine($"{i1}}}");
                sb.AppendLine("}");

                files.AddFile(new File
                {
                    Name = childModel.Plural.Value + $"Controller.relate{parentModel.Singular.Value}.cs",
                    Content = sb.ToString(),
                    CanOverwrite = Settings.SupportRegen
                });
            }
            return files;
        }
    }
}