using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Modeller.Interfaces;

namespace InterfaceClass
{
    public class GeneratorDetails : IMetadata
    {
        public Version Version => new Version(2, 1, 0, 0);

        public string Name => "Feature Interface";

        public string Description => "Build a Feature Interface used in Feature options";

        public Type EntryPoint => typeof(Generator);

        public IEnumerable<Type> SubGenerators => new Collection<Type>() { };
    }
}
