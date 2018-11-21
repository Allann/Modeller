using Hy.Modeller.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace Hy.Modeller.Generator
{
    public class PackageService
    {
        private readonly IPackageFileLoader _loader;
        private string _target;
        private IDictionary<string, IEnumerable<Package>> _items = new Dictionary<string, IEnumerable<Package>>();

        public PackageService()
        {
            Target = string.Empty;
            _loader = new PackageFileLoader();
        }

        public PackageService(IPackageFileLoader loader)
        {
            _loader = loader ?? throw new System.ArgumentNullException(nameof(loader));
        }

        public string Target
        {
            get => string.IsNullOrWhiteSpace(_target) ? Defaults.Target : _target;
            set => _target = value;
        }

        public string Folder { get; set; } = Directory.GetCurrentDirectory();

        public void Refresh()
        {
            var d = new DirectoryInfo(Folder);
            if (!d.Exists && string.IsNullOrWhiteSpace(Target)) return;

            var path = Path.Combine(d.FullName, Target + ".json");
            if (_loader.TryLoad(path, out var packages))
            {
                if (!_items.ContainsKey(Target))
                    _items.Add(Target, packages);
            }
        }

        public IEnumerable<Package> Items
        {
            get
            {
                if (!_items.ContainsKey(Target))
                    Refresh();
                return _items[Target];
            }
        }
    }
}