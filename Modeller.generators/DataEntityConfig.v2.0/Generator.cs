using System;
using Modeller;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace DataEntityConfig
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
            if (!_model.IsEntity())
                return null;

            var mappings = new FileGroup
            {
                Path = "EntityMappings"
            };

            if (Settings.SupportRegen)
                mappings.AddFile((IFile)new ConfigUser(Settings, _module, _model).Create());
            mappings.AddFile((IFile)new ConfigGenerated(Settings, _module, _model).Create());

            return mappings;
        }
    }
}