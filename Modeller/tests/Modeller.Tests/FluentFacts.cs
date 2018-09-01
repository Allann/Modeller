using FluentAssertions;
using Modeller.Models;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Modeller.Tests
{
    public class FluentFacts
    {
        private static string FileContent(string name)
        {
#if (NET461)
            var assembly = Assembly.GetExecutingAssembly();
#elif (NETCOREAPP)
            var assembly = Assembly.GetEntryAssembly();
#endif
            var names = assembly.GetManifestResourceNames();
            var resourceName = names.FirstOrDefault(f => f.Contains(name));
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

        [Fact]
        public void CanCreateModule()
        {
            var expected = new Models.Module { Company = "Jbssa" };
            expected.Project.SetName("freight rates");

            var actual = Fluent.Module.Create("freight rates").Build;

            actual.Should().BeOfType<Models.Module>().And.NotBeNull().And.BeEquivalentTo(expected);
            actual.Namespace.Should().Be("Jbssa.FreightRate");
        }

        [Fact]
        public void CanSetCompanyName()
        {
            var expected = new Models.Module { Company = "primo" };
            expected.Project.SetName("Optimity");

            var actual = Fluent.Module
                .Create("optimity")
                .CompanyName("Primo")
                .Build;

            actual.Should().BeOfType<Models.Module>().And.NotBeNull().And.BeEquivalentTo(expected);
            actual.Namespace.Should().Be("Primo.Optimity");
        }

        [Fact]
        public void CanSetFeature()
        {
            var expected = new Models.Module { Company = "primo" };
            expected.Project.SetName("Optimity");
            expected.Feature = new Name("small goods");

            var actual = Fluent.Module
                .Create("optimity")
                .CompanyName("Primo")
                .FeatureName("Small goods")
                .Build;

            actual.Should().BeOfType<Models.Module>().And.NotBeNull().And.BeEquivalentTo(expected);
            actual.Namespace.Should().Be("Primo.Optimity.SmallGood");
        }
    }
}