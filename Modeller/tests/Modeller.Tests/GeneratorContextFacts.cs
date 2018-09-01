using FluentAssertions;
using Modeller.Generator;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Modeller.Tests
{
    public static class TestDefaults
    {
        public static string BaseFolder => Path.Combine(Path.GetTempPath(), "Modeller");
    }

    [CollectionDefinition(name: "Context", DisableParallelization = true)]
    public static class GeneratorContextFacts
    {
        private static string BaseFolder => Path.Combine(TestDefaults.BaseFolder, "ContextTests");

        [Fact]
        public static void DefaultValuesInValidContext()
        {
            var local = new DirectoryInfo(BaseFolder);
            if (local.Exists)
                local.Delete(true);
            local.Create();
            local.CreateSubdirectory("Target");
            File.Copy("TestGenerator.dll", Path.Combine(local.FullName, "Target", "TestGenerator.dll"));
            Console.WriteLine(local.FullName);

            var moduleFile = new FileInfo(Path.Combine(local.FullName, "TestModule.json"));
            try
            {
                File.WriteAllText(moduleFile.FullName, "{\"company\":\"Jbssa\",\"project\":\"FreightRate\",\"models\":[{\"name\":\"Region\",\"key\":{\"fields\":[{\"name\":\"Id\",\"dataType\":\"UniqueIdentifier\",\"nullable\":false}]},\"fields\":[{\"name\":\"Code\",\"maxLength\":5,\"nullable\":false},{\"name\":\"Description\",\"maxLength\":500}],\"relationships\":[{\"left\":\"Region.Id\",\"leftType\":\"One\",\"right\":\"Country.RegionId\",\"rightType\":\"Many\"}]},{\"name\":\"Country\",\"key\":{\"fields\":[]},\"fields\":[],\"relationships\":[]}]}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var localOutput = new DirectoryInfo(Path.Combine(local.FullName, "GeneratorOutput"));

            //verify test conditions
            local.Exists.Should().BeTrue();
            moduleFile.Exists.Should().BeTrue();

            var context = new Context(moduleFile.FullName, local.FullName, "Test Generator", "Target", "1.0", null, "Region", localOutput.FullName);

            context.Module.Should().NotBeNull();
            context.Generator.Should().NotBeNull();
            context.Settings.Should().NotBeNull();
            context.Model.Should().NotBeNull();
            context.IsValid.Should().BeTrue();

            context.OutputPath.Should().Be(localOutput.FullName);
        }

        [Fact]
        public static void DefaultValuesInInvalidContext()
        {
            var folder = Defaults.LocalFolder;
            var exists = Directory.Exists(folder);

            //verify test conditions
            var context = new Context(null, null, null, null, null, null, null, null);

            context.Module.Should().BeNull();
            context.Generator.Should().BeNull();
            context.Settings.Should().NotBeNull();
            context.Model.Should().BeNull();
            context.IsValid.Should().BeFalse();

            context.OutputPath.Should().EndWith(Path.Combine("Visual Studio 2017", "Projects"));

            var list = new List<string> { "Missing the model file path and name.", "Context Module not defined.", "Context Generator not defined." };
            if (!exists)
                list.Add($"Local folder not found '{folder}'");

            context.ValidationMessages.Should().BeEquivalentTo(list);
        }
    }
}