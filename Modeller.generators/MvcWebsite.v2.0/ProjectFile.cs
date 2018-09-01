using System;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
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
            var project = new Project(_module.Namespace + ".Web") { Path = System.IO.Path.Combine("src", $"{_module.Namespace}.Web"), Id = Guid.NewGuid() };
            var files = new FileGroup();
            project.AddFileGroup(files);

            var sb = new StringBuilder();
            sb.AppendLine("<Project Sdk=\"Microsoft.NET.Sdk.Web\">");
            sb.AppendLine();
            sb.AppendLine("  <PropertyGroup>");
            sb.AppendLine("    <TargetFramework>netcoreapp2.1</TargetFramework>");
            sb.AppendLine($"    <AssemblyName>{project.Name}</AssemblyName>");
            sb.AppendLine($"    <RootNamespace>{project.Name}</RootNamespace>");
            sb.AppendLine("    <TypeScriptToolsVersion>2.5</TypeScriptToolsVersion>");
            sb.AppendLine("    <Company>JBS Australia</Company>");
            sb.AppendLine($"    <Product>{project.Name}</Product>");
            sb.AppendLine($"    <Description>Add a description for {project.Name}</Description>");
            sb.AppendLine("    <Copyright>Copyright © 2018 JBS Australia</Copyright>");
            sb.AppendLine("    <NeutralLanguage>en-AU</NeutralLanguage>");
            sb.AppendLine("    <Version>1.0.0</Version>");
            sb.AppendLine("    <AssemblyVersion>1.0.0.0</AssemblyVersion>");
            sb.AppendLine("    <FileVersion>1.0.0.0</FileVersion>");
            sb.AppendLine("  </PropertyGroup>");
            sb.AppendLine();
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine($"    <PackageReference Include=\"BuildBundlerMinifier\" Version=\"{Settings.GetPackageVersion("BuildBundlerMinifier")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Core.ExceptionTranslation\" Version=\"{Settings.GetPackageVersion("Core.ExceptionTranslation")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Core.HealthChecks.AspNetCore\" Version=\"{Settings.GetPackageVersion("Core.HealthChecks.AspNetCore")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Core.HealthChecks.Extensions\" Version=\"{Settings.GetPackageVersion("Core.HealthChecks.Extensions")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Core.Mvc\" Version=\"{Settings.GetPackageVersion("Core.Mvc")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"FluentValidation.AspNetCore\" Version=\"{Settings.GetPackageVersion("FluentValidation.AspNetCore")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Flurl.Http\" Version=\"{Settings.GetPackageVersion("Flurl.Http")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"IdentityServer4\" Version=\"{Settings.GetPackageVersion("IdentityServer4")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"IdentityModel\" Version=\"{Settings.GetPackageVersion("IdentityModel")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Microsoft.AspNet.WebApi.Client\" Version=\"{Settings.GetPackageVersion("Microsoft.AspNet.WebApi.Client")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Microsoft.AspNetCore.App\" Version=\"{Settings.GetPackageVersion("Microsoft.AspNetCore.App")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Microsoft.VisualStudio.Web.CodeGeneration.Design\" Version=\"{Settings.GetPackageVersion("Microsoft.VisualStudio.Web.CodeGeneration.Design")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Serilog.AspNetCore\" Version=\"{Settings.GetPackageVersion("Serilog.AspNetCore")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Serilog.Settings.Configuration\" Version=\"{Settings.GetPackageVersion("Serilog.Settings.Configuration")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Serilog.Sinks.Async\" Version=\"{Settings.GetPackageVersion("Serilog.Sinks.Async")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Serilog.Sinks.File\" Version=\"{Settings.GetPackageVersion("Serilog.Sinks.File")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Serilog.Sinks.Literate\" Version=\"{Settings.GetPackageVersion("Serilog.Sinks.Literate")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Serilog.Sinks.Seq\" Version=\"{Settings.GetPackageVersion("Serilog.Sinks.Seq")}\" />");
            sb.AppendLine($"    <PackageReference Include=\"Telerik.UI.for.AspNet.Core\" Version=\"{Settings.GetPackageVersion("Telerik.UI.for.AspNet.Core")}\" />");
            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine();
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine($"    <DotNetCliToolReference Include=\"BundlerMinifier.Core\" Version=\"{Settings.GetPackageVersion("BundlerMinifier.Core")}\" />");
            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine();
            sb.AppendLine("  <ItemGroup>");
            sb.AppendLine($"    <ProjectReference Include=\"..\\{_module.Namespace}.Dto\\{_module.Namespace}.Dto.csproj\" />");
            sb.AppendLine($"    <ProjectReference Include=\"..\\{_module.Namespace}.Security\\{_module.Namespace}.Security.csproj\" />");
            sb.AppendLine("  </ItemGroup>");
            sb.AppendLine("</Project>");
            files.AddFile(new File() { Name = project.Name + ".csproj", Content = sb.ToString() });
            return project;
        }
    }
}