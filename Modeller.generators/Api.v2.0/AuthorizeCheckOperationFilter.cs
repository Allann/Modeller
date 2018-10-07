using System;
using System.Text;
using Modeller.Core;
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
            var i1 = h.Indent(1);
            var i2 = h.Indent(2);
            var i3 = h.Indent(3);
            var i4 = h.Indent(4);
            var i5 = h.Indent(5);

            var sb = new StringBuilder();
            sb.AppendLine(((ISnippet)new Header.Generator(Settings, new GeneratorDetails()).Create()).Content);
            sb.AppendLine("using Microsoft.AspNetCore.Authorization;");
            sb.AppendLine("using Swashbuckle.AspNetCore.Swagger;");
            sb.AppendLine("using Swashbuckle.AspNetCore.SwaggerGen;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Reflection;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Api");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}public class AuthorizeCheckOperationFilter : IOperationFilter");
            sb.AppendLine($"{i1}{{");
            sb.AppendLine($"{i2}public void Apply(Operation operation, OperationFilterContext context)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}if (!context.ApiDescription.TryGetMethodInfo(out var mi)) return;");
            sb.AppendLine($"{i3}if (!mi.GetCustomAttributes().OfType<AuthorizeAttribute>().Any()) return;");
            sb.AppendLine();
            sb.AppendLine($"{i3}operation.Responses.Add(\"401\", new Response {{ Description = \"Unauthorized\" }});");
            sb.AppendLine($"{i3}operation.Responses.Add(\"403\", new Response {{ Description = \"Forbidden\" }});");
            sb.AppendLine();
            sb.AppendLine($"{i3}operation.Security = new List<IDictionary<string, IEnumerable<string>>>");
            sb.AppendLine($"{i3}{{");
            sb.AppendLine($"{i4}new Dictionary<string, IEnumerable<string>>");
            sb.AppendLine($"{i4}{{");
            sb.AppendLine($"{i5}{{ \"oauth2\", new [] {{ \"{_module.Project.Singular.LocalVariable}api\" }} }}");
            sb.AppendLine($"{i4}}}");
            sb.AppendLine($"{i3}}};");
            sb.AppendLine($"{i2}}}");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File { Name = "AuthorizeCheckOperationFilter.cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}