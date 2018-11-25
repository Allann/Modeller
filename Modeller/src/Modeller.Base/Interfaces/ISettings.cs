using System.Collections.Generic;

namespace Hy.Modeller.Interfaces
{
    public interface ISettings
    {
        bool SupportRegen { get; set; }

        string GetPackageVersion(string name);

        void RegisterPackage(Package package);

        void RegisterPackage(string name, string version);

        void RegisterPackages(IEnumerable<Package> packages);

        bool PackagesInitialised();
    }
}