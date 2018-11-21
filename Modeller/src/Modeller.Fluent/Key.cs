using System;

namespace Hy.Modeller.Fluent
{
    public static class Key
    {
        public static KeyBuilder Create(ModelBuilder modelBuilder, Models.Model model)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return new KeyBuilder(modelBuilder, model);
        }
    }
}
