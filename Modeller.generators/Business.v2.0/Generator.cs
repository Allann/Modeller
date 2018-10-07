using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace Business
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

            var services = new FileGroup
            {
                Path = "Services"
            };
            project.AddFileGroup(services);
            foreach (var item in _module.Models)
                project.AddFileGroup((IFileGroup)new BusinessService.Generator(Settings, _module, item).Create());

            var extensions = new FileGroup
            {
                Path = "Extensions"
            };
            project.AddFileGroup(extensions);
            foreach (var item in _module.Models)
                extensions.AddFile((IFile)new BusinessExtension.Generator(Settings, _module, item).Create());

            return project;
        }
    }
}