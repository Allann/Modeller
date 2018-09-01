using System;
using System.Text;
using Modeller;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
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

            files.AddFile((IFile)new ProgramFile(Settings, _module).Create());
            files.AddFile((IFile)new StartupFile(Settings, _module).Create());
            files.AddFile((IFile)new RouteNames(Settings, _module).Create());
            files.AddFile((IFile)new ErrorViewModel(Settings, _module).Create());
            files.AddFile((IFile)new ErrorHandler(Settings, _module).Create());
            files.AddFile((IFile)new Constants(Settings, _module).Create());
            files.AddFile((IFile)new AppSettings(Settings, _module).Create());
            files.AddFile((IFile)new AppSettingsJson(Settings, _module).Create());
            files.AddFile((IFile)new AppSettingsDevelopment(Settings, _module).Create());
            files.AddFile((IFile)new ServiceCollection(Settings,_module).Create());
            files.AddFile((IFile)new HomeController(Settings, _module).Create());

            project.AddFileGroup((IFileGroup)new Views(Settings, _module).Create());
            project.AddFileGroup((IFileGroup)new SharedViewModels(Settings, _module).Create());
            project.AddFileGroup((IFileGroup)new ViewComponents(Settings, _module).Create());
            project.AddFileGroup((IFileGroup)new Extensions(Settings, _module).Create());

            foreach (var item in _module.Models)
            {
                project.AddFileGroup((IFileGroup)new WebController.Generator(Settings, _module, item).Create());
                project.AddFileGroup((IFileGroup)new MvcWebViews.Generator(Settings, _module, item).Create());
                project.AddFileGroup((IFileGroup)new MvcWebViewModels.Generator(Settings, _module, item).Create());
            }

            return project;
        }
    }
}