using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace Api
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
            var files = new FileGroup();
            project.AddFileGroup(files);

            files.AddFile((IFile)new LaunchSettings(Settings, _module).Create());
            files.AddFile((IFile)new AppSettings(Settings, _module).Create());
            files.AddFile((IFile)new AppSettingsDev(Settings, _module).Create());
            files.AddFile((IFile)new AppSettingsDevelopment(Settings, _module).Create());
            files.AddFile((IFile)new ProgramFile(Settings, _module).Create());
            files.AddFile((IFile)new StartupFile(Settings, _module).Create());

            files.AddFile((IFile)new AuthorizeCheckOperationFilter(Settings, _module).Create());
            files.AddFile((IFile)new ServiceCollection(Settings, _module).Create());

            var controllers = new FileGroup
            {
                Path = "Controllers"
            };
            project.AddFileGroup(controllers);
            var mappers = new FileGroup
            {
                Path = "Mappings"
            };
            project.AddFileGroup(mappers);
            var options = new FileGroup
            {
                Path = "Options"
            };
            project.AddFileGroup(options);

            foreach (var item in _module.Models)
            {
                controllers.AddFile((IFile)new ApiController.Generator(Settings, _module, item).Create());
                mappers.AddFile((IFile)new ApiMapping.Generator(Settings, _module, item).Create());
                options.AddFile((IFile)new ApiOption.Generator(Settings, _module, item).Create());
            }

            return project;
        }
    }
}