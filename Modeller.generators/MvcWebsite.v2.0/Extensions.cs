using System;
using Modeller;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class Extensions : IGenerator
    {
        private readonly Module _module;
        
        public Extensions(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var files = new FileGroup
            {
                Path = "Extensions"
            };

            files.AddFile((IFile)new ControllerErrorExtensions(Settings, _module).Create());

            foreach (var item in _module.Models)
            {
                if (item.IsEntity())
                {
                    files.AddFile( (IFile)new MvcWebExtensions.Generator(Settings, _module, item).Create());
                }
            }
            return files;
        }
    }
}