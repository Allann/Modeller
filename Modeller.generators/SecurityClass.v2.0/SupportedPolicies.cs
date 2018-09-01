using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace SecurityClass
{
    internal class SupportedPolicies : IGenerator
    {
        private readonly Module _module;

        public SupportedPolicies(ISettings settings, Module module)
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
            sb.AppendLine("    public static class SupportedPolicies");
            sb.AppendLine("    {");
            sb.AppendLine($"        public const string RootAddress = \"{_module.Namespace}.Web.\";");
            foreach (var model in _module.Models)
            {
                sb.AppendLine($"        public const string {model.Name.Plural.Value} = RootAddress + \"{model.Name.Plural.Value}\";");
            }
            sb.AppendLine("        // CodeGen: Policy item (do not remove)");
            sb.AppendLine();
            sb.AppendLine("        public const string Hangfire = RootAddress + \"Hangfire\";");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File { Name = "SupportedPolicies.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}