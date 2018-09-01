using Modeller.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MvcWebViews
{
    public class GeneratorDetails : IMetadata
    {
        public Version Version => new Version(2, 1, 0, 0);

        public string Name => "Mvc Web Views";

        public string Description => "Build a MVC web views class";

        public Type EntryPoint => null;

        public IEnumerable<Type> SubGenerators => new Collection<Type>();
    }
}
