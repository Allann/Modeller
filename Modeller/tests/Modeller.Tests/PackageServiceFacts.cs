using FluentAssertions;
using Hy.Modeller.Generator;
using System.IO;
using Xunit;

namespace Hy.Modeller.Tests
{
    public static class PackageServiceFacts
    {
        [Fact]
        public static void PackageService()
        {
            var ps = new PackageService();

            ps.Folder.Should().Be(Directory.GetCurrentDirectory());
            ps.Target.Should().Be(Targets.Default);
            ps.Items.Should().NotBeEmpty();
        }

        [Fact]
        public static void SettingsPackageDefault()
        {
            var s = new Settings();
            s.GetPackageVersion("Core").Should().Be("");
        }
    }
}