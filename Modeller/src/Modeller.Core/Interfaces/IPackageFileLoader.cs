using System.Collections.Generic;

namespace Hy.Modeller.Interfaces
{
    public interface IPackageFileLoader
    {
        IEnumerable<Package> Load(string filePath);

        bool TryLoad(string filePath, out IEnumerable<Package> packages);
    }
}