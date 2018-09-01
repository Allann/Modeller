using Humanizer;
using System;
using System.ComponentModel;

namespace Modeller.Fluent
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ModuleBuilder : FluentBase
    {
        public ModuleBuilder(Models.Module module)
        {
            Build = module ?? throw new ArgumentNullException(nameof(module));
        }

        public Models.Module Build { get; }

        public ModuleBuilder CompanyName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            Build.Company = name.Pascalize();
            return this;
        }

        public ModuleBuilder FeatureName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                Build.Feature = null;
            else
            {
                var x = new Models.Name(name);
                Build.Feature = x;
            }
            return this;
        }

        public ModelBuilder AddModel(string name)
        {
            var model = Model.Create(this, name);
            Build.Models.Add(model.Instance);
            return model;
        }
    }
}
