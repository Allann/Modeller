using System;
using Modeller.Interfaces;
using Modeller.Models;
using Modeller.Outputs;

namespace MvcWebsite
{
    internal class Views : IGenerator
    {
        private readonly Module _module;

        public Views(ISettings settings, Module module)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _module = module ?? throw new ArgumentNullException(nameof(module));
        }

        public ISettings Settings { get; }

        public IOutput Create()
        {
            var output = new FileGroup
            {
                Path = "Views"
            };
            output.AddFile((IFile)new ViewImports(Settings, _module).Create());
            output.AddFile((IFile)new ViewStart(Settings, _module).Create());

            output.AddFile((IFile)new SharedViewCreateHeader(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewDeleteButton(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewDeleteDialog(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewDetailHeader(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewEditHeader(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewErrorDialog(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewErrorForm(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewFooter(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewHeader(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewIndexHeader(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewKeyword(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewLayout(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewLoadingPane(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewMenu(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewQuickFilter(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewSidebar(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewToastrBuilder(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewValidationScriptsPartial(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewValidationSummary(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewError(Settings, _module).Create());
            output.AddFile((IFile)new SharedView404(Settings, _module).Create());

            output.AddFile((IFile)new SharedViewComponentPageTitle(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewComponentTitle(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewComponentUser(Settings, _module).Create());
            output.AddFile((IFile)new SharedViewComponentVersion(Settings, _module).Create());

            return output;
        }
    }
}