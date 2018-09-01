using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewKeyword : IGenerator
    {
        private readonly Module _module;

        public SharedViewKeyword(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("@model BaseQuickFilterViewModel");
            sb.AppendLine("");
            sb.AppendLine("<div class=\"form-group\">");
            sb.AppendLine("    <div class=\"input-group\">");
            sb.AppendLine("        <input type=\"text\" class=\"form-control\" autocomplete=\"off\" placeholder=\"Keyword Search\" onClick=\"this.select();\" id=\"keywordFilterInput\" name=\"Keyword\" value=\"@Model.Keyword\">");
            sb.AppendLine("        <div class=\"input-group-btn\">");
            sb.AppendLine("            <button class=\"btn btn-default\" type=\"submit\"><i class=\"fas fa-search\"></i></button>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");

            return new File
            {
                Name = "_Keyword.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
