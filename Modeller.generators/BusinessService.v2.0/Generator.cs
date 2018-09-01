using System;
using System.Linq;
using Modeller;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace BusinessClass
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
            if (!_model.IsEntity() || !_model.Fields.Any())
                return null;

            var files = new FileGroup
            {
                Path = System.IO.Path.Combine("Services", _model.Name.Plural.Value)
            };

            files.AddFile((IFile)new ReadService(Settings, _module, _model).Create());
            files.AddFile((IFile)new EditService(Settings, _module, _model).Create());
            files.AddFile((IFile)new ListProvider(Settings, _module, _model).Create());
            files.AddFile((IFile)new FeatureService(Settings, _module, _model).Create());

            return files;
        }
    }
}