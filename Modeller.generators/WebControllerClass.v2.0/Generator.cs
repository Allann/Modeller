using System;
using Modeller;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace WebController
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

        public IOutput Create()
        {
            if (!_model.IsEntity())
                return null;

            var files = new FileGroup
            {
                Path = "Controllers"
            };

            files.AddFile((IFile)new CrudController(Settings, _module, _model).Create());
            files.AddFile((IFile)new PartialSidebarController(Settings, _module, _model).Create());
            if (new PartialRelateController(Settings, _module, _model).Create() is IFileGroup relate)
            {
                foreach (var file in relate.Files)
                {
                    files.AddFile(file);
                }
            }
            return files;
        }

        public ISettings Settings { get; }
    }
}