using Modeller.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MvcWebViewModels
{
    public class GeneratorDetails : IMetadata
    {
        public Version Version => new Version(2, 1, 0, 0);

        public string Name => "Mvc Web ViewModels";

        public string Description => "Build a Mvc web view models class";

        public Type EntryPoint => null;

        public IEnumerable<Type> SubGenerators => new Collection<Type>();
    }
}
