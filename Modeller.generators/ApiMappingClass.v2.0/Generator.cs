using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace ApiMapping
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
            var files = new FileGroup { Path = "Mappings" };

            files.AddFile((IFile)new Profile(Settings, _module, _model).Create());
            files.AddFile((IFile)new Mapper(Settings, _module, _model).Create());

            return files;
        }
    }
}