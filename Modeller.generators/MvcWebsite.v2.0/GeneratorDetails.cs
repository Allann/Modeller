using Modeller.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MvcWebsite
{
    public class GeneratorDetails : IMetadata
    {
        public Version Version => new Version(2, 1, 0, 0);

        public string Name => "Mvc Web Project";

        public string Description => "Build an Mvc Website project";

        public Type EntryPoint => typeof(Generator);

        public IEnumerable<Type> SubGenerators => new Collection<Type>() { typeof(MvcWebSiteProgram.Generator), typeof(WebController.Generator), typeof(MvcWebViews.Generator), typeof(MvcWebViewModels.Generator) };
    }
}
