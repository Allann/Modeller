using FluentAssertions;
using Modeller.Generator;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace Modeller.Tests
{
    [CollectionDefinition(name: "Updater", DisableParallelization = true)]
    public class UpdaterFacts
    {
        [Fact]
        public static void RefreshWithNonexistingServerFolder()
        {
            var outputValue = new StringBuilder();
            var updater = new Updater("X", "TestGenerators", "Y", output: s => outputValue.AppendLine(s));

            updater.Refresh();

            var expectedBeginning = "Updating generator files for target: Y" + Environment.NewLine + "Server Folder: X" + Environment.NewLine + "Local Folder: TestGenerators" + Environment.NewLine + "Overwrite: False" + Environment.NewLine + "Server Folder '";
            var expectedEnding = "\\X' not found." + Environment.NewLine + "Update failed. Files affected: 0" + Environment.NewLine;
            outputValue.ToString().Should().StartWith(expectedBeginning);
            outputValue.ToString().Should().EndWith(expectedEnding);
        }

        [Fact]
        public static void UpdaterDefaults()
        {
            var updater = new Updater(null, null, null);

            updater.LocalFolder.Should().Be(Defaults.LocalFolder);
            updater.ServerFolder.Should().Be(Defaults.ServerFolder);
            updater.Target.Should().Be(Defaults.Target);
            updater.Overwrite.Should().BeFalse();
            updater.Verbose.Should().BeFalse();
        }

        private readonly string _testServerFolder = Path.Combine(TestDefaults.BaseFolder, "Updater", "TestServerFolder");
        private readonly string _testLocalFolder = Path.Combine(TestDefaults.BaseFolder, "Updater", "TestLocalFolder");
        private readonly string _testGenerators = "TestGenerators";

        private void CleanupFromPreviousTests()
        {
            var server = new DirectoryInfo(_testServerFolder);
            var local = new DirectoryInfo(_testLocalFolder);

            if (server.Exists)
            {
                server.Delete(true);
            }
            server.Create();
            server.CreateSubdirectory(_testGenerators);
            if (local.Exists)
            {
                local.Delete(true);
            }
        }

        private void AddServerFile()
        {
            var serverTarget = new DirectoryInfo(Path.Combine(_testServerFolder, _testGenerators));
            var serverFile = Path.Combine(serverTarget.FullName, "File1.txt");
            File.WriteAllText(serverFile, "test file");
        }

        private void UpdateServerFile()
        {
            var serverTarget = new DirectoryInfo(Path.Combine(_testServerFolder, _testGenerators));
            var serverFile = Path.Combine(serverTarget.FullName, "File1.txt");
            File.WriteAllText(serverFile, "test file updated");
        }

        [Fact]
        public void RefreshLocalThatDoesNotExist()
        {
            // arrange
            var outputValue = new StringBuilder();
            void output(string s) => outputValue.AppendLine(s);

            CleanupFromPreviousTests();
            AddServerFile();

            var server = new DirectoryInfo(_testServerFolder);
            var local = new DirectoryInfo(_testLocalFolder);
            var updater = new Updater(server.FullName, local.FullName, _testGenerators, output: output);

            var serverTarget = new DirectoryInfo(Path.Combine(_testServerFolder, _testGenerators));
            var localTarget = new DirectoryInfo(Path.Combine(_testLocalFolder, _testGenerators));

            // verify test conditions
            serverTarget.GetFiles().Length.Should().Be(1);
            local.Exists.Should().BeFalse();

            // act
            updater.Refresh();

            // assert
            localTarget.Exists.Should().BeTrue();
            var dir = local.GetDirectories()[0];
            dir.Name.Should().Be(_testGenerators);
            dir.GetFiles().Length.Should().Be(1);
        }

        [Fact]
        public void RefreshLocalWithoutOverwriteDoesNotReplace()
        {
            // arrange
            var outputValue = new StringBuilder();
            void output(string s) => outputValue.AppendLine(s);
            var server = new DirectoryInfo(_testServerFolder);
            var local = new DirectoryInfo(_testLocalFolder);
            var updater = new Updater(server.FullName, local.FullName, _testGenerators, output: output);

            CleanupFromPreviousTests();
            AddServerFile();
            updater.Refresh();

            var serverTarget = new DirectoryInfo(Path.Combine(_testServerFolder, _testGenerators));
            var localTarget = new DirectoryInfo(Path.Combine(_testLocalFolder, _testGenerators));

            UpdateServerFile();

            // verify test conditions
            serverTarget.GetFiles().Length.Should().Be(1);
            local.Exists.Should().BeTrue();

            // act
            updater.Refresh();

            // assert
            var file = new FileInfo(Path.Combine(localTarget.FullName, "File1.txt"));
            using (var stream = file.OpenText())
            {
                stream.ReadToEnd().Should().Be("test file");
            }
        }

        [Fact]
        public void RefreshLocalWithOverwriteWillReplace()
        {
            // arrange
            var outputValue = new StringBuilder();
            void output(string s) => outputValue.AppendLine(s);
            var server = new DirectoryInfo(_testServerFolder);
            var local = new DirectoryInfo(_testLocalFolder);
            var updater = new Updater(server.FullName, local.FullName, _testGenerators, overwrite: true, output: output);

            CleanupFromPreviousTests();
            AddServerFile();
            updater.Refresh();

            var serverTarget = new DirectoryInfo(Path.Combine(_testServerFolder, _testGenerators));
            var localTarget = new DirectoryInfo(Path.Combine(_testLocalFolder, _testGenerators));

            UpdateServerFile();

            // verify test conditions
            serverTarget.GetFiles().Length.Should().Be(1);
            local.Exists.Should().BeTrue();

            // act
            updater.Refresh();

            // assert
            var file = new FileInfo(Path.Combine(localTarget.FullName, "File1.txt"));
            using (var stream = file.OpenText())
            {
                stream.ReadToEnd().Should().Be("test file updated");
            }
        }

        [Fact]
        public void VerboseOutput()
        {
            // arrange
            var outputValue = new StringBuilder();
            void output(string s) => outputValue.AppendLine(s);
            CleanupFromPreviousTests();
            AddServerFile();

            var server = new DirectoryInfo(_testServerFolder);
            var local = new DirectoryInfo(_testLocalFolder);
            var localTarget = new DirectoryInfo(Path.Combine(_testLocalFolder, _testGenerators));

            var updater = new Updater(server.FullName, local.FullName, _testGenerators, output: output, verbose: true);

            updater.Refresh();

            var actual = outputValue.ToString();
            actual.Should().Contain("creating " + localTarget.FullName);
            actual.Should().Contain("copying File1.txt to TestGenerators");
            actual.Should().Contain("Update completed successfully. Files affected: 1");
        }
    }
}