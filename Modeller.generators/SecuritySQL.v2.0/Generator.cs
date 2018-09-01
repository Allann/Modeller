using System;
using System.Collections.Generic;
using System.Text;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace SecuritySQL
{
    public class Generator : IGenerator
    {
        private readonly Module _module;

        public Generator(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var fileGroup = new FileGroup { Path = "sql" };

            var sb = new StringBuilder();
            sb.AppendLine($"-- Delete existing permissions and claims for {_module.Project.Singular.Display}");
            sb.AppendLine($"DELETE FROM Asap.[Application] WHERE [Name] LIKE '{_module.Namespace}'");
            sb.AppendLine($"DELETE FROM Asap.Module WHERE [Name] LIKE '{_module.Namespace}%'");
            sb.AppendLine($"DELETE FROM Asap.Permission WHERE [Name] LIKE '{_module.Namespace}%';");
            sb.AppendLine($"DELETE FROM Asap.Claim WHERE [Name] LIKE '{_module.Namespace}%';");
            sb.AppendLine();

            sb.AppendLine($"-- Create the application for {_module.Project.Singular.Display}");
            var applicationId = Guid.NewGuid();
            sb.AppendLine($"INSERT INTO Asap.Application (Id, [Name], [Description], IsActive, Created, CreatedBy, Modified, ModifiedBy) VALUES ('{applicationId}','{_module.Namespace}','Add a description for {_module.Project.Singular.Display}',1,SYSDATETIMEOFFSET(), 'CodeGen', SYSDATETIMEOFFSET(), 'CodeGen');");

            sb.AppendLine($"-- Create the website and api modules for {_module.Project.Singular.Display}");
            var webId = Guid.NewGuid();
            var apiId = Guid.NewGuid();
            sb.AppendLine($"INSERT INTO Asap.Module (Id, [Name], [Description], ApplicationId, Timeout, IsActive, Created, CreatedBy, Modified, ModifiedBy) VALUES ('{webId}','{_module.Namespace}.Web','Add a description for {_module.Namespace} website', '{applicationId}',3600,1,SYSDATETIMEOFFSET(), 'CodeGen', SYSDATETIMEOFFSET(), 'CodeGen');");
            sb.AppendLine($"INSERT INTO Asap.Module (Id, [Name], [Description], ApplicationId, Timeout, IsActive, Created, CreatedBy, Modified, ModifiedBy) VALUES ('{apiId}','{_module.Namespace}.Api','Add a description for {_module.Namespace} api services', '{applicationId}',60,1,SYSDATETIMEOFFSET(), 'CodeGen', SYSDATETIMEOFFSET(), 'CodeGen');");

            sb.AppendLine($"-- Create all claims for {_module.Project.Singular.Display}");
            var claims = new Dictionary<string, Guid>();
            foreach (var model in _module.Models)
            {
                claims.Add(model.Name.Singular.Value, Guid.NewGuid());
                sb.AppendLine("INSERT INTO Asap.Claim (Id, [Name], [Description], ClaimValueType, AllowMultipleInstances, AlwaysIncludeInIdToken, IsActive, IsResourceValue, IsRoleValue, IsUserValue, Created, CreatedBy, Modified, ModifiedBy) " +
                    $"VALUES('{claims[model.Name.Singular.Value]}', '{_module.Namespace}.{model.Name.Singular.Value}', 'Provide access to {_module.Project.Singular.Display} {model.Name.Singular.Display}', 'Static', 0, 0, 1, 1, 0, 0, SYSDATETIMEOFFSET(), 'CodeGen', SYSDATETIMEOFFSET(), 'CodeGen');");
            }
            sb.AppendLine();

            sb.AppendLine($"-- Create permissions for {_module.Project.Singular.Display}");
            var permissions = new Dictionary<string, Guid>();
            foreach (var model in _module.Models)
            {
                permissions.Add(model.Name.Singular.Value, Guid.NewGuid());
                sb.AppendLine($"INSERT INTO Asap.Permission (Id, [Name], [Description], IsActive, ModuleId, Created, CreatedBy, Modified, ModifiedBy) VALUES ('{permissions[model.Name.Singular.Value]}', '{_module.Namespace}.{model.Name.Singular.Value}', 'Provide access to {_module.Project.Singular.Display} {model.Name.Singular.Display}', 1, '{webId}', SYSDATETIMEOFFSET(), 'CodeGen', SYSDATETIMEOFFSET(), 'CodeGen')");
            }
            sb.AppendLine();

            sb.AppendLine($"-- Create permission/claim associations for {_module.Project.Singular.Display}");
            foreach (var model in _module.Models)
            {
                sb.AppendLine($"INSERT INTO Asap.PermissionClaims (PermissionId, ClaimId) VALUES ('{claims[model.Name.Singular.Value]}', '{permissions[model.Name.Singular.Value]}')");
            }
            sb.AppendLine();

            fileGroup.AddFile(new File() { Name = "security.sql", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen });

            sb.Clear();
            sb.AppendLine("# SQL Statements#");
            sb.AppendLine("This sql provides all the necessay data for ASAP security.");
            sb.AppendLine("Use them to create the default data required by the application to operate.");
            sb.AppendLine();
            sb.AppendLine("_Note_ you will still need to add roles manually.");
            fileGroup.AddFile(new File { Name = "ReadMe.md", Content = sb.ToString(), CanOverwrite = Settings.SupportRegen });

            return fileGroup;
        }
    }
}