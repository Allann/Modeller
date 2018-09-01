using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class ErrorViewModel : IGenerator
    {
        private readonly Module _module;

        public ErrorViewModel(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using IdentityServer4.Models;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Web.ViewModels");
            sb.AppendLine("{");
            sb.AppendLine("    public class ErrorViewModel");
            sb.AppendLine("    {");
            sb.AppendLine("        public ErrorMessage Error { get; set; }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File() { Name = "ErrorViewModel.cs", Content = sb.ToString(), Path = System.IO.Path.Combine("ViewModels", "Home") };
        }
    }
}