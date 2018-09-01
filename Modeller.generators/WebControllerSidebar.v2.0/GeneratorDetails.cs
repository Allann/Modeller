using Modeller.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WebControllerSidebar
{
    public class GeneratorDetails : IMetadata
    {
        public Version Version => new Version(2, 1, 0, 0);

        public string Name => "Mvc Controller Sidebar";

        public string Description => "Build an Mvc Web Controller sidebar";

        public Type EntryPoint => null;

        public IEnumerable<Type> SubGenerators => new Collection<Type>();
    }
}
