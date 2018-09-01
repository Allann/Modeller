using System;
using System.Collections.Generic;

namespace Modeller.Interfaces
{
    public interface IMetadata
    {
        Version Version { get; }

        string Name { get; }

        string Description { get; }

        Type EntryPoint { get; }

        IEnumerable<Type> SubGenerators { get; }
    }
}
