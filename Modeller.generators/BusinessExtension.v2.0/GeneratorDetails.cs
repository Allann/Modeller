using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Modeller.Interfaces;

namespace BusinessExtension
{
    public class GeneratorDetails : IMetadata
    {
        public Version Version => new Version(2, 1, 0, 0);

        public string Name => "Business Extensions";

        public string Description => "Build an Business Extension class";

        public Type EntryPoint => typeof(Generator);

        public IEnumerable<Type> SubGenerators => new Collection<Type>() { };
    }
}