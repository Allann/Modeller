using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebViews
{
    internal class InfoPartialView : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public InfoPartialView(ISettings settings, Module module, Model related)
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
            sb.AppendLine("<div class=\"panel panel-default jbs-infoPanel\">");
            sb.AppendLine("    <div class=\"row\">");
            sb.AppendLine($"        <partial name=\"_{_model.Name.Singular.Value}Content\" />");
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");

            var file = new File
            {
                Name = $"_{_model.Name.Singular.Value}Info.cshtml",
                Content = sb.ToString(),
                Path = "Shared",
                CanOverwrite = Settings.SupportRegen
            };
            return file;
        }
    }
}