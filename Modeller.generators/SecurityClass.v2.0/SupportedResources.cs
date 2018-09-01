using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace SecurityClass
{
    internal class SupportedResources : IGenerator
    {
        private readonly Module _module;

        public SupportedResources(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using Jbssa.Core.Security;");
            sb.AppendLine("using Microsoft.AspNetCore.Builder;");
            sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Security");
            sb.AppendLine("{");
            sb.AppendLine("    public static class SupportedResources");
            sb.AppendLine("    {");
            foreach (var model in _module.Models)
            {
                sb.AppendLine($"        public static DomainResource {model.Name.Singular.Value} {{ get; }} = new DomainResource(SupportedClaimTypes.{model.Name.Singular.Value});");
            }
            sb.AppendLine("        // CodeGen: Resource item (do not remove)");
            sb.AppendLine();
            sb.AppendLine("        public static DomainResource Hangfire { get; } = new DomainResource(SupportedClaimTypes.Hangfire);");
            sb.AppendLine("        public static DomainResource MyDetails { get; } = new DomainResource(SupportedClaimTypes.MyDetails);");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File { Name = "SupportedResources.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}