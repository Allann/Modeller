using FluentAssertions;
using Hy.Modeller.Generator;
using Hy.Modeller.Outputs;
using System.Text;
using Xunit;

namespace Hy.Modeller.Tests
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

        [Fact]
        public static void TestGeneratorLoader()
        {
            var sourceModel = "d:\\Dev\\securityportal_model.json";
            var localFolder = "d:\\Repos\\Modeller.Generators\\Generators";
            var generator = "MVCSolution";
            var target = Defaults.Target;
            var version = Defaults.Version.ToString();
            string settingFile = null;
            string modelName = null;
            var output = "d:\\dev\\test";

            var sb = new StringBuilder();

            var context = new Context(sourceModel, localFolder, generator, target, version, settingFile, modelName, output, output: s => sb.AppendLine(s));
            var codeGenerator = new CodeGenerator(context, s => sb.AppendLine(s), true);
            var presenter = new Creator(context, s => sb.AppendLine(s), true);
            presenter.Create(codeGenerator.Create());

            var consoleOutput = sb.ToString();
            consoleOutput.Should().Contain("Generation complete");
        }
    }
}