using Modeller.Extensions;
using Modeller.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace Modeller.Generator
{
    internal class PackageFileLoader : IPackageFileLoader
    {
        public IEnumerable<Package> Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Package file '{filePath}' does not exist.");
            }

            string packages;
            using (var reader = File.OpenText(filePath))
            {
                packages = reader.ReadToEnd();
            }
            return packages.FromJson<IEnumerable<Package>>();
        }

        public bool TryLoad(string filePath, out IEnumerable<Package> packages)
        {
            try
            {
                packages = Load(filePath);
                return true;
            }
            catch
            {
                packages = null;
                return false;
            }
        }
    }
}
