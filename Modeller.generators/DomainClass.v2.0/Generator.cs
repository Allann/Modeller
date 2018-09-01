using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace DomainClass
{
    public class Generator : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public Generator(ISettings settings, Module module, Model model)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var files = new FileGroup();

            if (Settings.SupportRegen)
                files.AddFile((IFile)new DomainUser(Settings, _module, _model).Create());
            files.AddFile((IFile)new DomainGenerated(Settings, _module, _model).Create());
            files.AddFile((IFile)new DomainAction(Settings, _module, _model).Create());
            files.AddFile((IFile)new DomainRelate(Settings, _module, _model).Create());

            return files;
        }
    }
}