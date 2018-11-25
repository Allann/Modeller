using Hy.Modeller.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Hy.Modeller
{
    public class Settings : ISettings
    {
        public bool SupportRegen { get; set; } = true;

        public string GetPackageVersion(string name)
        {
            if (Context.Packages == null || !Context.Packages.Any())
                return "";
            var found = Context.Packages.FirstOrDefault(p => string.Equals(p.Name, name));
            return found == null ? "" : found.Version;
        }

        public void RegisterPackage(string name, string version) => RegisterPackage(new Package(name, version));

        public void RegisterPackage(Package package)
        {
            if (package == null || string.IsNullOrWhiteSpace(package.Name) || string.IsNullOrWhiteSpace(package.Version))
                return;

            var packages = Context.Packages.Where(p => p.Name == package.Name);
            if (!packages.Any())
            {
                Context.Packages.Add(package);
                return;
            }
            if (packages.Count() == 1)
            {
                packages.First().Version = package.Version;
                return;
            }
        }

        public void RegisterPackages(IEnumerable<Package> packages)
        {
            foreach (var item in packages)
            {
                RegisterPackage(item);
            }
        }

        public bool PackagesInitialised() => Context.Packages.Any();

        public GeneratorContext Context { get; } = new GeneratorContext();
    }
}