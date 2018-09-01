using Modeller.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MvcWebSiteProgram
{
    public class GeneratorDetails : IMetadata
    {
        public Version Version => new Version(2, 1, 0, 0);

        public string Name => "Mvc Website Program Class";

        public string Description => "Build an Mvc Website Program class";

        public Type EntryPoint => typeof(Generator);

        public IEnumerable<Type> SubGenerators => new Collection<Type>() { };
    }
}
