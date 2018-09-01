using Modeller.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Property
{
    public class GeneratorDetails : IMetadata
    {
        public Version Version => new Version(2,1,0,0);

        public string Name => "C# Property";

        public string Description => "Build a C# property";

        public Type EntryPoint => null;

        public IEnumerable<Type> SubGenerators => new Collection<Type>();
    }
}
