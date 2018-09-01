using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace Dto
{
    internal class ProjectFile : IGenerator
    {
        private readonly Module _module;

        public ProjectFile(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var project = new Project(_module.Namespace + ".Dto") { Path = System.IO.Path.Combine("src", $"{_module.Namespace}.Dto"), Id = Guid.NewGuid() };
            var files = new FileGroup();
            project.AddFileGroup(files);

            var sb = new StringBuilder();
            sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
            sb.AppendLine();
            sb.AppendLine("  <PropertyGroup>");
            sb.AppendLine("    <TargetFramework>netstandard2.0</TargetFramework>");
            sb.AppendLine("  </PropertyGroup>");
            sb.AppendLine();
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine($"    <PackageReference Include=\"Core\" Version=\"{Settings.GetPackageVersion("Core")}\" />");
            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine();
            sb.AppendLine("</Project>");

            var projectFile = new File { Name = project.Name + ".csproj", Content = sb.ToString() };
            files.AddFile(projectFile);

            return project;
        }
    }
}