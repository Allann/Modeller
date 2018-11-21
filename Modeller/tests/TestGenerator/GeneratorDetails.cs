using Hy.Modeller.Interfaces;
using System;
using System.Collections.Generic;

namespace TestGenerator
{
    public class GeneratorDetails : IMetadata
    {
        public Version Version => new Version(1,0);

        public string Name => "Test Generator";

        public string Description => "A Test generator used for testing";

        public Type EntryPoint => typeof(Generator);

        public IEnumerable<Type> SubGenerators => new List<Type>();
    }
}
