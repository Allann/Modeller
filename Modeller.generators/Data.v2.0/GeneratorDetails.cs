using Modeller.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Data
{
    public class GeneratorDetails : IMetadata
    {
        public Version Version => new Version(2, 1, 0, 0);

        public string Name => "EF Data Project";

        public string Description => "Build an Data project using Entity Framework Core";

        public Type EntryPoint => typeof(Generator);

        public IEnumerable<Type> SubGenerators => new Collection<Type>() { typeof(DbContext.Generator), typeof(DataEntityConfig.Generator) };
    }
}
