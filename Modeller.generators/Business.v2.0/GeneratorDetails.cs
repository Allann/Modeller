using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Modeller.Interfaces;

namespace Business
{
    public class GeneratorDetails : IMetadata
    {
        public Version Version => new Version(2, 1, 0, 0);

        public string Name => "Business Project";

        public string Description => "Build an Business project targetting Asp.NET Core 2.1";

        public Type EntryPoint => typeof(Generator);

        public IEnumerable<Type> SubGenerators => new Collection<Type>()
        {
            typeof(BusinessClass.Generator),
            typeof(BusinessExtension.Generator)
        };
    }
}