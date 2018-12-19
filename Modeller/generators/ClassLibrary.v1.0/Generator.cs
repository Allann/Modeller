using System;
using Hy.Modeller.Interfaces;
using Hy.Modeller.Models;
using Hy.Modeller.Outputs;

namespace ClassLibrary
{
    public class Generator : IGenerator
    {
        private readonly Module _module;
        private readonly Model _model;

        public Generator(ISettings settings, Module module, Model model = null)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
            _model = model;
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            if (_model != null)
                return ClassGenerator.Create(_module, _model, Settings);

            var output = new FileGroup();
            foreach (var model in _module.Models)
            {
                output.AddFile(ClassGenerator.Create(_module, model, Settings));
            }
            return output;
        }
    }
}