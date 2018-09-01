using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewDeleteButton : IGenerator
    {
        private readonly Module _module;

        public SharedViewDeleteButton(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            //sb.AppendLine("<div authorize jbs-auth-resource=\"@Model.Authorization.ResourceName\" jbs-auth-permission=\"Delete\" class=\"form-group form-BtnGroup\">");
            sb.AppendLine("<div class=\"form-group form-BtnGroup\">");
            sb.AppendLine("    <hr />");
            sb.AppendLine("    <a class=\"pull-right btn btn-outline btn-danger\" href=\"javascript: openDeleteDialog();\" id=\"detailForm-BtnDelete\"><i class=\"far fa-trash-alt\"></i> Delete</a>");
            sb.AppendLine("</div>");

            return new File
            {
                Name = "_DeleteButton.cshtml",
                Path = "Shared",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
