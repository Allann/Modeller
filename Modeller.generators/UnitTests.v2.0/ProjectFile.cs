using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace UnitTests
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
            var project = new Project(_module.Namespace + ".UnitTests") { Path = System.IO.Path.Combine("src", $"{_module.Namespace}.UnitTests"), Id = Guid.NewGuid() };
            var files = new FileGroup();
            project.AddFileGroup(files);

            var sb = new StringBuilder();
            sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk\">");
            sb.AppendLine();
            sb.AppendLine("  <PropertyGroup>");
            sb.AppendLine("    <TargetFramework>netcoreapp2.1</TargetFramework>");
            sb.AppendLine("    <IsPackable>false</IsPackable>");
            sb.AppendLine("  </PropertyGroup>");
            sb.AppendLine();
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine("    <PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"15.8.0\" />");
            sb.AppendLine("    <PackageReference Include=\"Moq\" Version=\"4.9.0\" />");
            sb.AppendLine("    <PackageReference Include=\"xunit\" Version=\"2.4.0\" />");
            sb.AppendLine("    <PackageReference Include=\"xunit.runner.visualstudio\" Version=\"2.4.0\">");
            sb.AppendLine("      <PrivateAssets>all</PrivateAssets>");
            sb.AppendLine("      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>");
            sb.AppendLine("    </PackageReference>");
            sb.AppendLine("    <PackageReference Include=\"FluentAssertions\" Version=\"5.4.1\" />");
            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine();
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine($"    <ProjectReference Include=\"..\\..\\src\\{_module.Namespace}.Api\\{_module.Namespace}.Api.csproj\" />");
            sb.AppendLine($"    <ProjectReference Include=\"..\\..\\src\\{_module.Namespace}.Business\\{_module.Namespace}.Business.csproj\" />");
            sb.AppendLine($"    <ProjectReference Include=\"..\\..\\src\\{_module.Namespace}.Data\\{_module.Namespace}.Data.csproj\" />");
            sb.AppendLine($"    <ProjectReference Include=\"..\\..\\src\\{_module.Namespace}.Domain\\{_module.Namespace}.Domain.csproj\" />");
            sb.AppendLine($"    <ProjectReference Include=\"..\\..\\src\\{_module.Namespace}.Security\\{_module.Namespace}.Security.csproj\" />");
            sb.AppendLine($"    <ProjectReference Include=\"..\\..\\src\\{_module.Namespace}.Web\\{_module.Namespace}.Web.csproj\" />");
            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine("</Project>");

            files.AddFile(new File { Name = project.Name + ".csproj", Content = sb.ToString() });

            return project;
        }
    }
}