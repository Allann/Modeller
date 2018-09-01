using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace Api
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
            var project = new Project(_module.Namespace + ".Api") { Path = System.IO.Path.Combine("src", $"{_module.Namespace}.Api"), Id = Guid.NewGuid() };
            var files = new FileGroup();
            project.AddFileGroup(files);

            var sb = new StringBuilder();
            sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk.Web\">");
            sb.AppendLine();
            sb.AppendLine("  <PropertyGroup>");
            sb.AppendLine("    <TargetFramework>netcoreapp2.1</TargetFramework>");
            sb.AppendLine("  </PropertyGroup>");
            sb.AppendLine();
            sb.AppendLine("  <PropertyGroup Condition=\"'$(Configuration)|$(Platform)'=='Debug|AnyCPU'\">");
            sb.AppendLine("    <DocumentationFile></DocumentationFile>");
            sb.AppendLine("  </PropertyGroup>");
            sb.AppendLine();
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine($"    <PackageReference Include=\"AutoMapper.Extensions.Microsoft.DependencyInjection\" Version=\"{Settings.GetPackageVersion("AutoMapper.Extensions.Microsoft.DependencyInjection")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Core.ExceptionTranslation\" Version=\"{Settings.GetPackageVersion("Core.ExceptionTranslation")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Core.HealthChecks.AspNetCore\" Version=\"{Settings.GetPackageVersion("Core.HealthChecks.AspNetCore")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Core.HealthChecks.Extensions.SqlServer\" Version=\"{Settings.GetPackageVersion("Core.HealthChecks.Extensions.SqlServer")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Core.Mvc\" Version=\"{Settings.GetPackageVersion("Core.Mvc")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"FluentValidation.AspNetCore\" Version=\"{Settings.GetPackageVersion("FluentValidation.AspNetCore")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"IdentityServer4.AccessTokenValidation\" Version=\"{Settings.GetPackageVersion("IdentityServer4.AccessTokenValidation")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Microsoft.AspNetCore.App\" Version=\"{Settings.GetPackageVersion("Microsoft.AspNetCore.App")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Swashbuckle.AspNetCore\" Version=\"{Settings.GetPackageVersion("Swashbuckle.AspNetCore")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Swashbuckle.AspNetCore.Swagger\" Version=\"{Settings.GetPackageVersion("Swashbuckle.AspNetCore.Swagger")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Serilog.AspNetCore\" Version=\"{Settings.GetPackageVersion("Serilog.AspNetCore")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Serilog.Settings.Configuration\" Version=\"{Settings.GetPackageVersion("Serilog.Settings.Configuration")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Serilog.Sinks.Async\" Version=\"{Settings.GetPackageVersion("Serilog.Sinks.Async")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Serilog.Sinks.File\" Version=\"{Settings.GetPackageVersion("Serilog.Sinks.File")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Serilog.Sinks.Literate\" Version=\"{Settings.GetPackageVersion("Serilog.Sinks.Literate")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Serilog.Sinks.Seq\" Version=\"{Settings.GetPackageVersion("Serilog.Sinks.Seq")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Microsoft.VisualStudio.Web.CodeGeneration.Design\" Version=\"{Settings.GetPackageVersion("Microsoft.VisualStudio.Web.CodeGeneration.Design")}\" PrivateAssets=\"All\" />");
            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine();
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine($"    <ProjectReference Include=\"..\\{_module.Namespace}.Business\\{_module.Namespace}.Business.csproj\" />");
            sb.AppendLine($"    <ProjectReference Include=\"..\\{_module.Namespace}.Interface\\{_module.Namespace}.Interface.csproj\" />");
            sb.AppendLine($"    <ProjectReference Include=\"..\\{_module.Namespace}.Security\\{_module.Namespace}.Security.csproj\" />");
            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine("</Project>");

            files.AddFile(new File { Name = project.Name + ".csproj", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen });

            return project;
        }
    }
}