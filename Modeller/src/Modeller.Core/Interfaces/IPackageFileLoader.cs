using System.Collections.Generic;

namespace Modeller.Interfaces
{
    internal interface IPackageFileLoader
    {
        IEnumerable<Package> Load(string filePath);

        bool TryLoad(string filePath, out IEnumerable<Package> packages);
    }
}
