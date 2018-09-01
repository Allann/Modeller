using FluentAssertions;
using Modeller.Generator;
using Xunit;

namespace Modeller.Tests
{
    public static class ContextFacts
    {
        [Fact]
        public static void CanCreateContextUsingDefaults()
        {
            var context = new Context(null, null, null, null, null, null, null, null);

            context.Should().NotBeNull();
            context.Folder.Should().Be(Defaults.LocalFolder);
            context.Target.Should().Be(Defaults.Target);
            context.Version.Should().Be(Defaults.Version);
        }

        //[Fact]
        //public void ValidationPassesWithCorrectValues()
        //{
        //    var path = Path.Combine(Directory.GetCurrentDirectory(), "TestJsonFiles", "FreightRates.json");
        //    var context = new Context(path, null, "Domain", null, null, null, null, null);

        //    context.Validate();
        //    context.IsValid.Should().BeTrue();
        //    context.ValidationMessages.Should().HaveCount(0);
        //}

        [Fact]
        public static void ValidationFailsWhenModuleFileMissing()
        {
            var context = new Context(null, null, null, null, null, null, null, null);

            context.Validate();
            context.IsValid.Should().BeFalse();
        }
    }
}