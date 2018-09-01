using FluentAssertions;
using Modeller.Generator;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace Modeller.Tests
{
    [CollectionDefinition(name: "Presenter", DisableParallelization = true)]
    public class ListPresenterFacts
    {
        private readonly string _testLocalFolder = Path.Combine(TestDefaults.BaseFolder, "Presenter", "TestPresenterFolder");
        private const string _testGenerators = "TestGenerators";

        private void CleanupFromPreviousTests()
        {
            var local = new DirectoryInfo(_testLocalFolder);
            if (local.Exists)
            {
                local.Delete(true);
            }

            var target = new DirectoryInfo(Path.Combine(_testLocalFolder, _testGenerators));
            target.Create();
            File.Copy("TestGenerator.dll", Path.Combine(target.FullName, "TestGenerator.dll"));
        }

        [Fact]
        public void PresenterOutput()
        {
            var outputValue = new StringBuilder();
            void output(string s, bool b)
            { if (b) { outputValue.AppendLine(s); } else { outputValue.Append(s); } }

            CleanupFromPreviousTests();

            var local = new DirectoryInfo(_testLocalFolder);
            var target = new DirectoryInfo(Path.Combine(local.FullName, _testGenerators));

            // verify test conditions
            target.Exists.Should().BeTrue();
            target.GetFiles().Length.Should().Be(1);

            var presenter = new Presenter(_testLocalFolder, _testGenerators, output);

            // act
            presenter.Display();

            // assert
            var folder = Path.Combine(_testLocalFolder, _testGenerators);
            var actual = outputValue.ToString();
            actual.Should().Be("Available generators" + Environment.NewLine + $"  location: {folder}" + Environment.NewLine + "TestGenerator | Test Generator | 1.0" + Environment.NewLine);
        }

        [Fact]
        public void PresenterOutputVerbose()
        {
            var outputValue = new StringBuilder();

            CleanupFromPreviousTests();

            var local = new DirectoryInfo(_testLocalFolder);
            var target = new DirectoryInfo(Path.Combine(local.FullName, _testGenerators));

            // verify test conditions
            target.Exists.Should().BeTrue();
            target.GetFiles().Length.Should().Be(1);

            var presenter = new Presenter(_testLocalFolder, _testGenerators, (s, b) => { if (b) { outputValue.AppendLine(s); } else { outputValue.Append(s); } });

            // act
            presenter.Display(true);

            // assert
            var folder = Path.Combine(_testLocalFolder, _testGenerators);
            var actual = outputValue.ToString();
            actual.Should().Be("Available generators" + Environment.NewLine + $"  location: {folder}" + Environment.NewLine + "TestGenerator | Test Generator | 1.0 | A Test generator used for testing" + Environment.NewLine);
        }
    }
}
