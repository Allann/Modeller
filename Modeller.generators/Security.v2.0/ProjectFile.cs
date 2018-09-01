using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace Security
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
            var project = new Project(_module.Namespace + ".Security") { Path = System.IO.Path.Combine("src", $"{_module.Namespace}.Security"), Id = Guid.NewGuid() };
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
            sb.AppendLine($"    <PackageReference Include=\"Core.Security\" Version=\"{Settings.GetPackageVersion("Core.Security")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"IdentityServer4.AspNetIdentity\" Version=\"{Settings.GetPackageVersion("IdentityServer4.AspNetIdentity")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"IdentityServer4.EntityFramework\" Version=\"{Settings.GetPackageVersion("IdentityServer4.EntityFramework")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Microsoft.AspNetCore.Identity\" Version=\"{Settings.GetPackageVersion("Microsoft.AspNetCore.Identity")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Microsoft.AspNetCore.Identity.EntityFrameworkCore\" Version=\"{Settings.GetPackageVersion("Microsoft.AspNetCore.Identity.EntityFrameworkCore")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Microsoft.AspNetCore.Server.IISIntegration\" Version=\"{Settings.GetPackageVersion("Microsoft.AspNetCore.Server.IISIntegration")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Microsoft.EntityFrameworkCore.SqlServer\" Version=\"{Settings.GetPackageVersion("Microsoft.EntityFrameworkCore.SqlServer")}\" />");
            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine();
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine($"    <ProjectReference Include=\"..\\{_module.Namespace}.Domain\\{_module.Namespace}.Domain.csproj\" />");
            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine("</Project>");

            var projectFile = new File { Name = project.Name + ".csproj", Content = sb.ToString() };
            files.AddFile(projectFile);

            return project;
        }
    }
}