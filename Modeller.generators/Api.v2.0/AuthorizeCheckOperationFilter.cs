using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace Api
{
    internal class AuthorizeCheckOperationFilter : IGenerator
    {
        private readonly Module _module;

        public AuthorizeCheckOperationFilter(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using FluentValidation.AspNetCore;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using Microsoft.AspNetCore.Authorization;");
            sb.AppendLine("using Swashbuckle.AspNetCore.Swagger;");
            sb.AppendLine("using Swashbuckle.AspNetCore.SwaggerGen;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Api");
            sb.AppendLine("{");
            sb.AppendLine("    public class AuthorizeCheckOperationFilter : IOperationFilter");
            sb.AppendLine("    {");
            sb.AppendLine("        public void Apply(Operation operation, OperationFilterContext context)");
            sb.AppendLine("        {");
            sb.AppendLine();
            sb.AppendLine("            var requiredScopes = context.MethodInfo");
            sb.AppendLine("                .GetCustomAttributes(true)");
            sb.AppendLine("                .OfType<AuthorizeAttribute>()");
            sb.AppendLine("                .Select(attr => attr.Policy)");
            sb.AppendLine("                .Distinct();");
            sb.AppendLine("            if (requiredScopes.Any())");
            sb.AppendLine("            {");
            sb.AppendLine("                operation.Responses.Add(\"401\", new Response { Description = \"Unauthorized\" });");
            sb.AppendLine("                operation.Responses.Add(\"403\", new Response { Description = \"Forbidden\" });");
            sb.AppendLine();
            sb.AppendLine("                operation.Security = new List<IDictionary<string, IEnumerable<string>>>");
            sb.AppendLine("                {");
            sb.AppendLine("                    new Dictionary<string, IEnumerable<string>>");
            sb.AppendLine("                    {");
            sb.AppendLine("                        { \"oauth2\", requiredScopes }");
            sb.AppendLine("                    }");
            sb.AppendLine("                };");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return new File { Name = "AuthorizeCheckOperationFilter.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}