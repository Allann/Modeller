using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class ViewComponentUser: IGenerator
    {
        private readonly Module _module;
        
        public ViewComponentUser(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Jbssa.Core.Mvc.ViewModels;");
            sb.AppendLine("using Jbssa.Core.Security;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Web.ViewComponents");
            sb.AppendLine("{");
            sb.AppendLine("    public class UserViewComponent : ViewComponent");
            sb.AppendLine("    {");
            sb.AppendLine("        public async Task<IViewComponentResult> InvokeAsync()");
            sb.AppendLine("        {");
            sb.AppendLine("            var user = await Task.FromResult(HttpContext?.User);");
            sb.AppendLine("            if (user != null)");
            sb.AppendLine("                return View(new UserViewModel { UserId = user.GetName() });");
            sb.AppendLine("            return View();");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File
            {
                Name = "UserViewComponent.cs",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}