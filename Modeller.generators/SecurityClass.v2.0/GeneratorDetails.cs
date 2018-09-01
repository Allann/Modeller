using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Modeller.Interfaces;

namespace SecurityClass
{
    public class GeneratorDetails : IMetadata
    {
        public Version Version => new Version(2, 1, 0, 0);

        public string Name => "Security Extension";

        public string Description => "Build a Security extension class";

        public Type EntryPoint => typeof(Generator);

        public IEnumerable<Type> SubGenerators => new Collection<Type>();
    }
}