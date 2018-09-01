using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewModelMyDetails : IGenerator
    {
        private readonly Module _module;
        
        public SharedViewModelMyDetails(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations;");
            sb.AppendLine("using System.Security.Claims;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Web.ViewModels.Shared");
            sb.AppendLine("{");
            sb.AppendLine("    public class MyDetailsViewModel");
            sb.AppendLine("    {");
            sb.AppendLine("        [Display(Name = \"User\")]");
            sb.AppendLine("        public string Username { get; set; }");
            sb.AppendLine();
            sb.AppendLine("        public IEnumerable<Claim> Claims { get; internal set; }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File
            {
                Name = "MyDetailsViewModel.cs",
                Content = sb.ToString(),
                CanOverwrite = Settings.SupportRegen
            };
        }
    }
}
