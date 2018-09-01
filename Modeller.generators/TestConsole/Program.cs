using System;
using System.Collections.Generic;
using System.Linq;
using Modeller;
using Modeller.Interfaces;

namespace TestConsole
{
    internal class Program
    {
        private static Modeller.Models.Module CreateModule()
        {
            //todo: edit this method to create your test module
            return Modeller.Fluent.Module
                .Create("Asap")
                .CompanyName("Jbssa")
                .AddModel("Application")
                    .WithDefaultKey()
                    .IsAuditable(true)
                    .AddField("Name")
                        .BusinessKey(true)
                        .MaxLength(100)
                        .Nullable(false)
                        .Build
                    .AddField("Description")
                        .MaxLength(1000)
                        .Build
                    .AddField("IsActive")
                        .DataType(Modeller.Models.DataTypes.Bool)
                        .Nullable(false)
                        .Default("true")
                        .Build
                    .AddRelationship()
                        .Relate("Application.Id", "Module.ApplicationId")
                        .Build
                    .Build
                .AddModel("Module")
                    .WithDefaultKey()
                    .IsAuditable(true)
                    .AddField("ApplicationId")
                        .DataType(Modeller.Models.DataTypes.UniqueIdentifier)
                        .Nullable(false)
                        .Build
                    .AddField("Name")
                        .BusinessKey(true)
                        .MaxLength(100)
                        .Nullable(false)
                        .Build
                    .AddField("Description")
                        .MaxLength(1000)
                        .Build
                    .AddField("Timeout")
                        .DataType(Modeller.Models.DataTypes.Number)
                        .Nullable(false)
                        .Build
                    .AddRelationship()
                        .Relate("Module.ApplicationId", "Application.Id", Modeller.Models.RelationShipTypes.Many, Modeller.Models.RelationShipTypes.One)
                        .Relate("Module.Id", "EnviornmentModule.ModuleId")
                        .Build
                    .Build
                .AddModel("Environment")
                    .WithDefaultKey()
                    .IsAuditable(true)
                    .AddField("Name")
                        .BusinessKey(true)
                        .MaxLength(100)
                        .Nullable(false)
                        .Build
                    .AddRelationship()
                        .Relate("Environment.Id", "EnviornmentModule.EnvironmentId")
                        .Build
                    .Build
                .AddModel("EnvironmentModule")
                    .WithKey()
                        .AddField("ModuleId").DataType(Modeller.Models.DataTypes.UniqueIdentifier).Build
                        .AddField("EnvironmentId").DataType(Modeller.Models.DataTypes.UniqueIdentifier).Build
                        .Build
                    .AddRelationship()
                        .Relate("Module.Id", "EnvironmentModule.ModuleId").Build
                    .AddRelationship()
                        .Relate("Environment.Id", "EnvironmentModule.EnvironmentId").Build
                    .Build
                .Build;
        }

        private static IGenerator CreateGenerator()
        {
            // todo : edit this line to change the generator to use
            var module = CreateModule();
            var model = module.Models.First(m => m.Name.Singular.Value == "Application");
            return new DbContext.Generator(Settings, module);
        }

        // todo: change the settings only if necessary
        private static ISettings Settings => new Settings();

        // Don't change code below here

        private static void Main(string[] args)
        {
            var g = CreateGenerator();
            var output = g.Create();
            IEnumerable<IFile> files = null;
            //if (output is IProject project)
            //    files = project.FileGroups
            //else
            if (output is IFileGroup fileGroup)
                files = fileGroup.Files;
            else if (output is IFile file)
                files = new List<IFile> { file };
            else if (output is ISnippet snippet)
            {
                files = new List<IFile> { new Modeller.Outputs.File { Name = snippet.Name, Path = "c:\temp", Content = snippet.Content } };
            }

            if (files == null)
            {
                Console.WriteLine(output);
            }
            else
            {
                var c = Console.ForegroundColor;
                foreach (var file in files)
                {
                    Console.WriteLine(file.FullName);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(file.Content);
                    Console.ForegroundColor = c;
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Press Enter to finish.");
            Console.ReadLine();
        }
    }
}