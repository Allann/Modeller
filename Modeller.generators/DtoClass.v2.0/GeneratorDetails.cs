using Modeller.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DtoClass
{
    public class GeneratorDetails : IMetadata
    {
        public Version Version => new Version(2, 1, 0, 0);

        public string Name => "Dto Class";

        public string Description => "Build a Data Transfer Object class";

        public Type EntryPoint => typeof(Generator);

        public IEnumerable<Type> SubGenerators => new Collection<Type>() { typeof(Property.Generator), typeof(Header.Generator) };
    }
}
