using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Modeller.Interfaces;

namespace BusinessService
{
    public class GeneratorDetails : IMetadata
    {
        public Version Version => new Version(2, 1, 0, 0);

        public string Name => "Read Service";

        public string Description => "Build an Read Service class";

        public Type EntryPoint => typeof(Generator);

        public IEnumerable<Type> SubGenerators => new Collection<Type>() { typeof(Header.Generator) };
    }
}