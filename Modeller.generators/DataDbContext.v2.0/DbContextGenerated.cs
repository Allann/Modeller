using System;
using System.Collections.Generic;
using System.Text;
using Modeller;
using Modeller.Core;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace DbContext
{
    internal class DbContextGenerated : IGenerator
    {
        private readonly Module _module;

        public DbContextGenerated(ISettings settings, Module module)
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

            var sb = new StringBuilder();
            sb.AppendLine(((ISnippet)new Header.Generator(Settings, new GeneratorDetails()).Create()).Content);
            if (!Settings.SupportRegen)
            {
                sb.AppendLine("using Jbssa.Core.Data;");
                sb.AppendLine("using Microsoft.AspNetCore.Http;");
            }
            sb.AppendLine($"using {_module.Namespace}.Domain;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_module.Namespace}.Data");
            sb.AppendLine("{");
            sb.Append($"{i1}");
            if (!Settings.SupportRegen)
                sb.Append("public ");
            sb.Append($"partial class {_module.Project.Singular.Value}DbContext");
            if (!Settings.SupportRegen)
                sb.Append(": DbContextBase");
            sb.AppendLine();
            sb.AppendLine($"{i1}{{");
            if (!Settings.SupportRegen)
            {
                sb.AppendLine($"{i2}public {_module.Project.Singular.Value}DbContext(DbContextOptions<{_module.Project.Singular.Value}DbContext> options, IHttpContextAccessor httpContextAccessor)");
                sb.AppendLine($"{i3}: base(options, httpContextAccessor)");
                sb.AppendLine($"{i2}{{ }}");
            }
            else
            {
                sb.AppendLine($"{i2}partial void ModelCreating(ModelBuilder modelBuilder);");
            }
            sb.AppendLine();
            sb.AppendLine($"{i2}protected override string DefaultSchema => \"{_module.Project.Singular.Value}\";");
            foreach (var model in _module.Models)
            {
                sb.AppendLine();
                sb.AppendLine($"{i2}public DbSet<{model.Name.Singular.Value}> {model.Name.Plural.Value} {{ get; set; }}");
            }
            sb.AppendLine($"{i2}// CodeGen: DbSet (do not remove)");
            sb.AppendLine($"{i2}protected override void OnModelCreating(ModelBuilder modelBuilder)");
            sb.AppendLine($"{i2}{{");
            sb.AppendLine($"{i3}base.OnModelCreating(modelBuilder);");
            if (Settings.SupportRegen)
            {
                sb.AppendLine();
                sb.AppendLine($"{i3}ModelCreating(modelBuilder);");
            }

            foreach (var model in _module.Models)
            {
                if (model.IsEntity() || model.Key.Fields.Count != 2)
                    continue;

                //  look for a One-to-Many-to-One relationship pair
                if (model.Relationships.Count >= 2)
                {
                    var found = new List<Relate>();
                    foreach (var relationship in model.Relationships)
                    {
                        var relate = relationship.GetDetails(model.Name);
                        if (relate.MatchType == RelationShipTypes.Many && relate.OtherType == RelationShipTypes.One)
                        {
                            found.Add(relate);
                        }
                    }
                    // found the two relationships
                    if (found.Count == 2)
                    {
                        sb.AppendLine();
                        sb.AppendLine($"{i3}// {found[0].Match.Singular.Value}");
                        sb.AppendLine($"{i3}modelBuilder.Entity<{found[0].Match.Singular.Value}>()");
                        sb.AppendLine($"{i4}.ToTable(\"{found[0].Match.Singular.Value}\")");
                        sb.AppendLine($"{i4}.HasKey(t => new {{ t.{found[0].MatchField.Singular.Value}, t.{found[1].MatchField.Singular.Value} }});");
                        sb.AppendLine();
                        sb.AppendLine($"{i3}modelBuilder.Entity<{found[0].Other.Singular.Value}>()");
                        sb.AppendLine($"{i4}.HasMany(a => a.{found[0].Match.Plural.Value})");
                        sb.AppendLine($"{i4}.WithOne(ma => ma.{found[0].Other.Singular.Value})");
                        sb.AppendLine($"{i4}.HasForeignKey(m => m.{found[0].MatchField.Singular.Value})");
                        sb.AppendLine($"{i4}.OnDelete(DeleteBehavior.Cascade);");
                        sb.AppendLine();
                        sb.AppendLine($"{i3}modelBuilder.Entity<{found[1].Other.Singular.Value}>()");
                        sb.AppendLine($"{i4}.HasMany(a => a.{found[1].Match.Plural.Value})");
                        sb.AppendLine($"{i4}.WithOne(ma => ma.{found[1].Other.Singular.Value})");
                        sb.AppendLine($"{i4}.HasForeignKey(m => m.{found[1].MatchField.Singular.Value})");
                        sb.AppendLine($"{i4}.OnDelete(DeleteBehavior.Cascade);");

                    }
                }
            }
            sb.AppendLine($"{i2}}}");
            sb.AppendLine($"{i1}}}");
            sb.AppendLine("}");

            var name = _module.Project.Singular.Value + "DbContext";
            if (Settings.SupportRegen)
                name += ".generated";
            return new File { Name = name + ".cs", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen };
        }
    }
}
