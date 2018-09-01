using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace Business
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
            var project = new Project(_module.Namespace + ".Business") { Path = System.IO.Path.Combine("src", $"{_module.Namespace}.Business"), Id = Guid.NewGuid() };
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
            sb.AppendLine($"    <PackageReference Include=\"Core.BusinessLogic\" Version=\"{Settings.GetPackageVersion("Core.BusinessLogic")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"EPPlus\" Version=\"{Settings.GetPackageVersion("EPPlus")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Flurl.Http\" Version=\"{Settings.GetPackageVersion("Flurl.Http")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Microsoft.AspNetCore.JsonPatch\" Version=\"{Settings.GetPackageVersion("Microsoft.AspNetCore.JsonPatch")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Microsoft.Extensions.Logging\" Version=\"{Settings.GetPackageVersion("Microsoft.Extensions.Logging")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"System.ComponentModel.Annotations\" Version=\"{Settings.GetPackageVersion("System.ComponentModel.Annotations")}\" />");
            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine();
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine($"    <ProjectReference Include=\"..\\{_module.Namespace}.Data\\{_module.Namespace}.Data.csproj\" />");
            sb.AppendLine($"    <ProjectReference Include=\"..\\{_module.Namespace}.Interface\\{_module.Namespace}.Interface.csproj\" />");
            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine("</Project>");

            files.AddFile(new File { Name = project.Name + ".csproj", Content = sb.ToString() });

            return project;
        }
    }
}