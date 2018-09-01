using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class RouteNames : IGenerator
    {
        private readonly Module _module;

        public RouteNames(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {_module.Namespace}.Web");
            sb.AppendLine("{");
            sb.AppendLine("    public static class RouteNames");
            sb.AppendLine("    {");
            foreach (var model in _module.Models)
            {
                sb.AppendLine($"        public const string {model.Name.Singular.Value}Lookup = nameof({model.Name.Singular.Value}Lookup);");
            }
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File { Name = "RouteNames.cs", Content = sb.ToString() };
        }
    }
}