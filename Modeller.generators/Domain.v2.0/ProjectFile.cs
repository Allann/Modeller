using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace Domain
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
            var project = new Project(_module.Namespace + ".Domain") { Path = System.IO.Path.Combine("src", $"{_module.Namespace}.Domain"), Id = Guid.NewGuid() };
            var files = new FileGroup();
            project.AddFileGroup(files);

            var sb = new StringBuilder();
            sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
            sb.AppendLine();
            sb.AppendLine("  <PropertyGroup>");
            sb.AppendLine("    <TargetFramework>netstandard2.0</TargetFramework>");
            sb.AppendLine($"    <RootNamespace>{project.Name}</RootNamespace>");
            sb.AppendLine("  </PropertyGroup>");
            sb.AppendLine();
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine($"    <PackageReference Include=\"Core\" Version=\"{Settings.GetPackageVersion("Core")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"FluentValidation\" Version=\"{Settings.GetPackageVersion("FluentValidation")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"System.ComponentModel.Annotations\" Version=\"{Settings.GetPackageVersion("System.ComponentModel.Annotations")}\" />");
            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine();
            sb.AppendLine("</Project>");

            var projectFile = new File { Name = project.Name + ".csproj", Content = sb.ToString() };
            files.AddFile(projectFile);

            return project;
        }
    }
}