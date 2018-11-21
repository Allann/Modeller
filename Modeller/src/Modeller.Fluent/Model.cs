using System;

namespace Hy.Modeller.Fluent
{
    public static class Model
    {
        public static ModelBuilder Create(ModuleBuilder module, string name)
        {
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(name);
            }

            var model = new Models.Model();
            model.Name.SetName(name);
            return new ModelBuilder(module, model);
        }
    }
}
