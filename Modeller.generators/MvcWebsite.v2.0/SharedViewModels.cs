using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class SharedViewModels : IGenerator
    {
        private readonly Module _module;
        
        public SharedViewModels(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var output = new FileGroup
            {
                Path = System.IO.Path.Combine("ViewModels", "Shared")
            };

            output.AddFile((IFile)new SharedViewModelAccessDenied(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewModelApplicationModel(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewModelErrorDialog(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewModelLoadingPane(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewModelMyDetails(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewModelNotFound(Settings, _module).Create());
            return output;
        }
    }
}
