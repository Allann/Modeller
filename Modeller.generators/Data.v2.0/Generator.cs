using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace Data
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
            var project = (IProject)new ProjectFile(Settings, _module).Create();
            project.AddFileGroup((IFileGroup)new DbContext.Generator(Settings, _module).Create());

            foreach (var item in _module.Models)
                project.AddFileGroup((IFileGroup)new DataEntityConfig.Generator(Settings, _module, item).Create());

            return project;
        }
    }
}