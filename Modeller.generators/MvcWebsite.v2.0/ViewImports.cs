using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class ViewImports : IGenerator
    {
        private readonly Module _module;

        public ViewImports(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("@using Jbssa.Core.Security");
            sb.AppendLine("@using Jbssa.Core.Mvc.ViewModels");
            sb.AppendLine("");
            sb.AppendLine($"@using {_module.Namespace}.Security");
            sb.AppendLine($"@using {_module.Namespace}.Dto");
            sb.AppendLine($"@using {_module.Namespace}.Web");
            sb.AppendLine($"@using {_module.Namespace}.Web.ViewModels.Shared");
            sb.AppendLine($"@using {_module.Namespace}.Web.Extensions");
            sb.AppendLine("");
            sb.AppendLine("@using System.Collections");
            sb.AppendLine("@using System.Threading.Tasks");
            sb.AppendLine("");
            sb.AppendLine("@using Kendo.Mvc.UI");
            sb.AppendLine("@using Microsoft.AspNetCore.Authorization");
            sb.AppendLine("");
            sb.AppendLine("@addTagHelper *, Kendo.Mvc");
            sb.AppendLine("@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers");
            sb.AppendLine("@addTagHelper Core.Security.AuthorizeTagHelper, Core.Security");

            return new File
            {
                Name = "_ViewImports.cshtml",
                Content = sb.ToString(),
                CanOverwrite=Settings.SupportRegen
            };
        }
    }
}