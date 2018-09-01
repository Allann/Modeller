using Modeller.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ApiMapping
{
    public class GeneratorDetails : IMetadata
    {
        public Version Version => new Version(2, 1, 0, 0);

        public string Name => "WebApi Mapping";

        public string Description => "Build an Api Mapping class targetting Asp.NET Core 2.1";

        public Type EntryPoint => typeof(Generator);

        public IEnumerable<Type> SubGenerators => new Collection<Type>() { };
    }
}
