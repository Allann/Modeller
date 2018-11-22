using Hy.Modeller.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Hy.Modeller.GeneratorBase
{
    public abstract class MetadataBase : IMetadata
    {
        public MetadataBase()
        {
            var vers = GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            if (Version.TryParse(vers, out Version result)) Version = result;
        }

        public Version Version { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract Type EntryPoint { get; }
        public abstract IEnumerable<Type> SubGenerators { get; }
    }
}
