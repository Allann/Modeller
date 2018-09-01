using Modeller.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MvcWebExtensions
{
    public class GeneratorDetails : IMetadata
    {
        public Version Version => new Version(2, 1, 0, 0);

        public string Name => "MVC Web Extensions";

        public string Description => "Build a MVC web extensions class";

        public Type EntryPoint => typeof(Generator);

        public IEnumerable<Type> SubGenerators => new Collection<Type>() { };
    }
}
