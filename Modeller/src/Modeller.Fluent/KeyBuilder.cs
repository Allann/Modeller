using System;
using System.ComponentModel;

namespace Hy.Modeller.Fluent
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class KeyBuilder : FluentBase
    {
        public KeyBuilder(ModelBuilder modelBuilder, Models.Model model)
        {
            Build = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
            Instance = model ?? throw new ArgumentNullException(nameof(model));
        }

        public ModelBuilder Build { get; }
        public Models.Model Instance { get; }

        public KeyFieldBuilder AddField(string name)
        {
            var field = Field.Create(this, name);
            Instance.Key.Fields.Add(field.Instance);
            return field;
        }
    }
}
