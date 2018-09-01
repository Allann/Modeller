using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Modeller.Interfaces;

namespace DomainClass
{
    public class GeneratorDetails : IMetadata
    {
        public Version Version => new Version(2, 1, 0, 0);

        public string Name => "Domain Class";

        public string Description => "Build a Domain class file group";

        public Type EntryPoint => typeof(Generator);

        public IEnumerable<Type> SubGenerators => new Collection<Type>() { typeof(Property.Generator), typeof(Header.Generator) };
    }
}