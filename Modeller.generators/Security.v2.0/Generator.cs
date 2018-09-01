using System;
using Modeller.Interfaces;
using Modeller.Models;

namespace Security
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
            var project = ((IProject)new ProjectFile(Settings, _module).Create());

            project.AddFileGroup((IFileGroup)new SecurityClass.Generator(Settings, _module).Create());
            project.AddFileGroup((IFileGroup)new SecuritySQL.Generator(Settings, _module).Create());

            return project;
        }
    }
}