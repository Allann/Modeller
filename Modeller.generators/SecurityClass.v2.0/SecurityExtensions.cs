using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace SecurityClass
{
    internal class SecurityExtensions : IGenerator
    {
        private readonly Module _module;

        public SecurityExtensions(ISettings settings, Module module)
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
            sb.AppendLine("    public static class SecurityExtensions");
            sb.AppendLine("    {");
            sb.AppendLine($"        public static IServiceCollection Add{_module.Project.Singular.Value}Policies(this IServiceCollection services)");
            sb.AppendLine("        {");
            sb.AppendLine("            services.AddAuthorization(options =>");
            sb.AppendLine("            {");
            foreach (var model in _module.Models)
            {
                sb.AppendLine($"                options.AddPolicy(SupportedPolicies.{model.Name.Plural.Value}, policy => policy");
                sb.AppendLine("                    .RequireAuthenticatedUser()");
                sb.AppendLine($"                    .RequireClaim(SupportedClaimTypes.{model.Name.Singular.Value}));");
            }
            sb.AppendLine("                // CodeGen: Policy item (do not remove)");
            sb.AppendLine();
            sb.AppendLine("            });");
            sb.AppendLine("            return services;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public static IApplicationBuilder UseCoreSecurity(this IApplicationBuilder app)");
            sb.AppendLine("        {");
            sb.AppendLine("            app.UseJbsSecurity();");
            sb.AppendLine("            app.UseAuthentication();");
            sb.AppendLine("            return app;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File { Name = "SecurityExtensions.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}