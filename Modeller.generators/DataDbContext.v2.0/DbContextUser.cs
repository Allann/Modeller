using System;
using System.Text;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace DbContext
{
    internal class DbContextUser : IGenerator
    {
        private readonly Module _module;

        public DbContextUser(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            if (!Settings.SupportRegen)
                return null;

            var i1 = h.Indent(1);
            var i2 = h.Indent(2);
            var i3 = h.Indent(3);
            var i4 = h.Indent(4);

            var sb = new StringBuilder();
            sb.AppendLine(((ISnippet)new Header.Generator(Settings, new GeneratorDetails()).Create()).Content);
            sb.AppendLine("using Jbssa.Core.Data;");
            sb.AppendLine("using Microsoft.AspNetCore.Http;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Data");
            sb.AppendLine("{");
            sb.AppendLine($"{i1}public class {_module.Project.Singular.Value}DbContext : DbContextBase");
            sb.AppendLine($"{i1}{{");

            sb.AppendLine($"{i2}public {_module.Project.Singular.Value}DbContext(DbContextOptions<{_module.Project.Singular.Value}DbContext> options, IHttpContextAccessor httpContextAccessor)");
            sb.AppendLine($"{i3}: base(options, httpContextAccessor)");
            sb.AppendLine($"{i2}{{ }}");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            return new File { Name = _module.Project.Singular.Value + "DbContext.cs", Content = sb.ToString(), CanOverwrite = false };
        }
    }
}
