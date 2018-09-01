using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class ViewComponentTitle : IGenerator
    {
        private readonly Module _module;
        
        public ViewComponentTitle(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine("");
            sb.AppendLine($"namespace {_module.Namespace}.Web.ViewComponents");
            sb.AppendLine("{");
            sb.AppendLine("    public class TitleViewComponent : ViewComponent");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly ViewModels.Shared.ApplicationModel _applicationModel;");
            sb.AppendLine("");
            sb.AppendLine("        public TitleViewComponent(ViewModels.Shared.ApplicationModel applicationModel)");
            sb.AppendLine("        {");
            sb.AppendLine("            _applicationModel = applicationModel;");
            sb.AppendLine("        }");
            sb.AppendLine("");
            sb.AppendLine("        public Task<IViewComponentResult> InvokeAsync()");
            sb.AppendLine("        {");
            sb.AppendLine("            return Task.FromResult<IViewComponentResult>(View(_applicationModel));");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File
            {
                Name = "TitleViewComponent.cs",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}