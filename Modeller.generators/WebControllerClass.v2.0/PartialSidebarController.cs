using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace WebController
{
    internal class PartialSidebarController : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;
        
        public PartialSidebarController(ISettings settings, Module module, Model model)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            if (!_model.Relationships.Any())
                return null;

            var i1 = h.Indent(1);
            var i2 = h.Indent(2);
            var i3 = h.Indent(3);
            var i4 = h.Indent(4);

            var fields = _model.Fields.ToList();
            fields.AddRange(_model.Key.Fields);
            var parentModel = _model.Name;

            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using Jbssa.Core.Mvc.ViewModels;");
            sb.AppendLine($"using {_module.Namespace}.Dto;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine("using Newtonsoft.Json;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Web.Controllers");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}partial class {parentModel.Plural.Value}Controller");
            sb.AppendLine($"{i1}{{");
            sb.Append($"{i2}partial void AddLinks(SidebarViewModel sidebar, {parentModel.Singular.Value}Response data = null");
            foreach (var item in _model.Relationships)
            {
                item.GetOther(parentModel, out var childType, out var childModel, out var childField);
                if (childType == RelationShipTypes.One)
                {
                    sb.Append($", Guid? {childModel.Singular.LocalVariable}{childField.Singular.Value} = null");
                }
            }
            sb.AppendLine(")");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}var {parentModel.Singular.LocalVariable}Id = data?.Id;");

            var defaultLinks = new List<string>();
            var detailLinks = new List<string>();
            foreach (var item in _model.Relationships)
            {
                item.GetMatch(parentModel, out var parentType, out var parentField);
                item.GetOther(parentModel, out var childType, out var childModel, out var childField);
                if (childType == RelationShipTypes.One)
                {
                    sb.AppendLine($"{i3}var {childModel.Singular.LocalVariable}Link = new ActionSidebarItem(\"lnk{childModel.Singular.Value}\", \"{childModel.Singular.Display} Details\", \"{childModel.Plural.Value}\", \"Detail\")");
                    sb.AppendLine($"{i3}{{");
                    sb.AppendLine($"{i4}Enabled = {childModel.Singular.LocalVariable}Id.HasValue");
                    sb.AppendLine($"{i3}}};");
                    sb.AppendLine($"{i3}{childModel.Singular.LocalVariable}Link.Parameters.Add(\"id\", {childModel.Singular.LocalVariable}Id.GetValueOrDefault().ToString());");
                    detailLinks.Add($"{childModel.Singular.LocalVariable}Link");
                }
                else if (childType == RelationShipTypes.Many)
                {
                    sb.AppendLine($"{i3}var {childModel.Singular.LocalVariable}Link = new ActionSidebarItem(\"lnk{childModel.Singular.Value}\", \"Manage {childModel.Plural.Display}\", \"{childModel.Plural.Value}\", \"For{parentModel.Singular.Value}\")");
                    sb.AppendLine($"{i3}{{");
                    sb.AppendLine($"{i4}Enabled = {parentModel.Singular.LocalVariable}Id.HasValue");
                    sb.AppendLine($"{i3}}};");
                    sb.AppendLine($"{i3}{childModel.Singular.LocalVariable}Link.Parameters.Add(\"{parentModel.Singular.LocalVariable}Id\", {parentModel.Singular.LocalVariable}Id.GetValueOrDefault().ToString());");
                    defaultLinks.Add($"{childModel.Singular.LocalVariable}Link");
                }
            }
            if (defaultLinks.Any())
                sb.AppendLine($"{i3}sidebar.AddLinks(\"default\", new[] {{ {string.Join(", ", defaultLinks.ToArray())} }});");
            if (detailLinks.Any())
                sb.AppendLine($"{i3}sidebar.AddLinks(\"detail\", new[] {{ {string.Join(", ", detailLinks.ToArray())} }});");

            sb.AppendLine($"{i2}}}");
            sb.AppendLine();
            sb.AppendLine($"{i2}[HttpPost]");
            sb.Append($"{i2}public IActionResult SidebarItems(string selectedData");
            foreach (var item in _model.Relationships)
            {
                item.GetOther(parentModel, out var childType, out var childModel, out var childField);
                if (childType == RelationShipTypes.One)
                {
                    sb.Append($", Guid? {childModel.Singular.LocalVariable}Id");
                }
            }
            sb.AppendLine(")");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}{parentModel.Singular.Value}Response data = null;");
            sb.AppendLine($"{i3}if (selectedData != null)");
            sb.AppendLine($"{i3}{{");
            sb.AppendLine($"{i4}data = JsonConvert.DeserializeObject<{parentModel.Singular.Value}Response>(selectedData);");
            sb.AppendLine($"{i3}}}");
            sb.AppendLine();
            sb.AppendLine($"{i3}var viewModel = new SidebarViewModel();");
            sb.Append($"{i3}AddLinks(viewModel, data");
            foreach (var item in _model.Relationships)
            {
                item.GetOther(parentModel, out var childType, out var childModel, out var childField);
                if (childType == RelationShipTypes.One)
                {
                    sb.Append($", {childModel.Singular.LocalVariable}Id");
                }
            }
            sb.AppendLine(");");
            sb.AppendLine($"{i3}return PartialView(\"_Sidebar\", viewModel);");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File
            {
                Name = _model.Name.Plural.Value + "Controller.sidebar.cs",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}