using System;
using Modeller;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebViewModels
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
            var output = new FileGroup
            {
                Path = System.IO.Path.Combine("ViewModels", _model.Name.Plural.Value)
            };

            if (!_model.IsEntity())
                return output;

            output.AddFile((IFile)new FilterViewModel(Settings, _module, _model).Create());
            output.AddFile((IFile)new CreateViewModel(Settings, _module, _model).Create());
            output.AddFile((IFile)new EditViewModel(Settings, _module, _model).Create());
            output.AddFile((IFile)new DetailViewModel(Settings, _module, _model).Create());

            return output;
        }
    }
}