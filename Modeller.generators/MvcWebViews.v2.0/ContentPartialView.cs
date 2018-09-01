using System;
using System.Text;
using Modeller;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebViews
{
    internal class ContentPartialView : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public ContentPartialView(ISettings settings, Module module, Model related)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _model = related ?? throw new ArgumentNullException(nameof(related));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"@model {_model.Name.Singular.Value}Response");
            sb.AppendLine();
            foreach (var field in _model.Key.Fields)
            {
                sb.AppendLine($"<input asp-for=\"{field.Name.Singular.Value}\" type=\"hidden\" />");
            }
            foreach (var field in _model.Fields)
            {
                sb.AppendLine($"<input asp-for=\"{field.Name.Singular.Value}\" type=\"hidden\" />");
            }
            sb.AppendLine();
            sb.AppendLine("<div class=\"col-sm-1 col-xs-4\">");
            sb.AppendLine($"    {_model.Name.Singular.Display}");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class=\"col-sm-3 col-xs-8\">");
            sb.AppendLine("    <div class=\"row\">");
            sb.AppendLine("        <div class=\"col-sm-10\">");
            var bk = _model.HasBusinessKey();
            if (bk != null)
            {
                sb.AppendLine($"            <strong>@Model.{bk.Name.Singular.Value}</strong>");
            }
            else
            {
                sb.AppendLine($"            <strong>@Model.ToString()</strong>");
            }
            if (_model.HasActive())
            {
                sb.AppendLine("            @if (!Model.IsActive)");
                sb.AppendLine("            {");
                sb.AppendLine("                <span class=\"label small label-warning superscript\">Inactive</span>");
                sb.AppendLine("            }");
            }
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");
            sb.AppendLine("@* Add extra data if needed.  Make sure all columns don't exceed 12 width");
            sb.AppendLine("<div class=\"col-sm-8 hidden-xs\">");
            sb.AppendLine("    @Model.Description");
            sb.AppendLine("</div>*@");

            var file = new File
            {
                Name = $"_{_model.Name.Singular.Value}Content.cshtml",
                Content = sb.ToString(),
                Path = "Shared",
                CanOverwrite = Settings.SupportRegen
            };
            return file;
        }
    }
}