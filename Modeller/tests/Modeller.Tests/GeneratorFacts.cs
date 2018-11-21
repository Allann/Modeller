using System.IO;
using System.Linq;
using System.Reflection;

namespace Hy.Modeller.Tests
{
    public class DomainGeneratorFacts
    {
        private static string FileContent(string name, bool regen)
        {
#if (NET461)
            var assembly = Assembly.GetExecutingAssembly();
#elif (NETCOREAPP)
            var assembly = Assembly.GetEntryAssembly();
#endif
            var names = assembly.GetManifestResourceNames();
            var src = regen ? "regen" : "single";
            var resourceName = names.FirstOrDefault(f => f.Contains(src + "." + name));
            if (resourceName == null)
            {
                return null;
            }

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static Models.Module TestModule()
        {
            var module =
                Fluent
                    .Module
                        .Create("Asap")
                        .AddModel("Claim")
                            .AddField("Name").MaxLength(200).BusinessKey(true).Nullable(false).Build
                            .AddField("Description").MaxLength(1000).Build
                            .AddField("ClaimValueType").MaxLength(200).Nullable(false).Build
                            .AddField("AllowMultipleInstances").DataType(Models.DataTypes.Bool).Nullable(false).Build
                            .AddField("AlwaysIncludeInIdToken").DataType(Models.DataTypes.Bool).Nullable(false).Build
                            .AddField("IsResourceValue").DataType(Models.DataTypes.Bool).Nullable(false).Build
                            .AddField("IsRoleValue").DataType(Models.DataTypes.Bool).Nullable(false).Build
                            .AddField("IsUserValue").DataType(Models.DataTypes.Bool).Nullable(false).Build
                            .AddField("IsActive").DataType(Models.DataTypes.Bool).Nullable(false).Default("true").Build
                        .Build
                    .Build;
            return module;
        }

        //[Theory]
        //[InlineData(true)]
        //[InlineData(false)]
        //public void GenerateDomainForModule(bool regen)
        //{
        //    var module = TestModule();
        //    var setting = new Settings() { SupportRegen = regen };
        //    var gen = new MvcSolution.Generator(setting, module);
        //    var solution = (ISolution)gen.Create();

        //    solution.Name.Should().Be("ASAP");
        //    ((Solution)solution).Namespace.Should().Be("Jbssa.ASAP");
        //    solution.Projects.Should().HaveCount(1);

        //    var domain = solution.Projects[0];
        //    domain.Name.Should().Be("Jbssa.ASAP.Domain");
        //    if(regen)
        //        domain.Files.Should().HaveCount(5);
        //    else
        //        domain.Files.Should().HaveCount(4);

        //    foreach (var file in domain.Files)
        //    {
        //        var content = FileContent(file.Name, gen.Settings.SupportRegen);
        //        content.Should().NotBeNull("embedded resource {0} shoule exist.", file.Name);
        //        file.Content.Should().Be(content, "file content invalid for {0}", file.Name);
        //    }
        //}
    }
}