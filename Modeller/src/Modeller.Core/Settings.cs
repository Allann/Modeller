using System.Collections.Generic;
using System.Linq;
using Modeller.Interfaces;

namespace Modeller
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

        public GeneratorContext Context { get; } = new GeneratorContext();
    }
}