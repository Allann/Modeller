using System;

namespace Hy.Modeller.Fluent
{
    public static class Field
    {
        public static FieldBuilder Create(ModelBuilder model, string name)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(name);
            }

            var field = new Models.Field(name);
            return new FieldBuilder(model, field);
        }

        public static KeyFieldBuilder Create(KeyBuilder model, string name)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(name);
            }

            var field = new Models.Field(name);
            return new KeyFieldBuilder(model, field);
        }

        public static IndexFieldBuilder Create(IndexBuilder model, string name)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(name);
            }

            var field = new Models.IndexField(name);
            return new IndexFieldBuilder(model, field);
        }
    }
}
