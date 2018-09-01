using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace DtoClass
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
            var files = new FileGroup
            {
                Path = _model.Name.Plural.Value
            };

            files.AddFile((IFile)new ResponseUser(Settings, _module, _model).Create());
            files.AddFile((IFile)new ResponseGenerated(Settings, _module, _model).Create());
            files.AddFile((IFile)new ListResponseUser(Settings, _module, _model).Create());
            files.AddFile((IFile)new ListResponseGenerated(Settings, _module, _model).Create());
            files.AddFile((IFile)new CreateRequestUser(Settings, _module, _model).Create());
            files.AddFile((IFile)new CreateRequestGenerated(Settings, _module, _model).Create());
            files.AddFile((IFile)new UpdateRequestUser(Settings, _module, _model).Create());
            files.AddFile((IFile)new UpdateRequestGenerated(Settings, _module, _model).Create());

            return files;
        }
    }
}