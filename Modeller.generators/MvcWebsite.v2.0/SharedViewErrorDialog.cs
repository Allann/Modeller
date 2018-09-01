using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewErrorDialog : IGenerator
    {
        private readonly Module _module;

        public SharedViewErrorDialog(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("@model ErrorDialogViewModel");
            sb.AppendLine();
            sb.AppendLine("<div class=\"modal fade\" id=\"divErrorDialog\" tabindex=\"-1\" role=\"dialog\">");
            sb.AppendLine("    <div class=\"modal-dialog\" role=\"document\">");
            sb.AppendLine("        <div class=\"modal-content\">");
            sb.AppendLine("            <div id=\"divErrorFormContent\">");
            sb.AppendLine("                <partial name=\"_ErrorForm\")/>");
            sb.AppendLine("            </div>");
            sb.AppendLine("            <div class=\"modal-footer\">");
            sb.AppendLine("                <a class=\"btn btn-outline btn-default\" data-dismiss=\"modal\">OK</a>");
            sb.AppendLine("            </div>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</div>");
            return new File
            {
                Name = "_ErrorDialog.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
