using Modeller.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MvcSolution
{
    public class GeneratorDetails : IMetadata
    {
        public Version Version => new Version(2, 1, 0, 0);

        public string Name => "MVC Solution";

        public string Description => "Build an MVC Solution using ASP.Net Core";

        public Type EntryPoint => typeof(Generator);

        public IEnumerable<Type> SubGenerators => new Collection<Type>()
        {
            typeof(Api.Generator),
            typeof(Business.Generator),
            typeof(Data.Generator),
            typeof(Domain.Generator),
            typeof(Dto.Generator),
            typeof(Interface.Generator),
            typeof(MvcWebsite.Generator),
            typeof(Security.Generator),
            typeof(UnitTests.Generator)
        };
    }
}
