using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace SecurityClass
{
    internal class ClaimTypes : IGenerator
    {
        private readonly Module _module;

        public ClaimTypes(ISettings settings, Module module)
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
            sb.AppendLine("    public static class SupportedClaimTypes");
            sb.AppendLine("    {");
            sb.AppendLine($"        public const string RootAddress = \"{_module.Namespace}.\";");
            foreach (var model in _module.Models)
            {
                sb.AppendLine($"        public const string {model.Name.Singular.Value} = RootAddress + \"{model.Name.Singular.Value}\";");
            }
            sb.AppendLine("        // CodeGen: ClaimType item (do not remove)");
            sb.AppendLine();
            sb.AppendLine("        public const string Hangfire = RootAddress + \"Hangfire\";");
            sb.AppendLine("        public const string MyDetails = RootAddress + \"MyDetails\";");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File { Name = "SupportedClaimTypes.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}