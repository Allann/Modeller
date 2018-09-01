using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class ViewComponents : IGenerator
    {
        private readonly Module _module;
        
        public ViewComponents(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var vc = new FileGroup
            {
                Path = "ViewComponents"
            };

            vc.AddFile((IFile)new ViewComponentPageTitle(Settings, _module).Create());
            vc.AddFile((IFile)new ViewComponentTitle(Settings, _module).Create());
            vc.AddFile((IFile)new ViewComponentUser(Settings, _module).Create());
            vc.AddFile((IFile)new ViewComponentVersion(Settings, _module).Create());

            return vc;
        }
    }

}