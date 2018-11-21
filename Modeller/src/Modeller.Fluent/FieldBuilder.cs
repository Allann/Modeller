using Hy.Modeller.Models;
using System;
using System.ComponentModel;

namespace Hy.Modeller.Fluent
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class FieldBuilder 
    {
        public FieldBuilder(ModelBuilder modelBuilder, Models.Field field)
        {
            Build = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
            Instance = field ?? throw new ArgumentNullException(nameof(field));
        }

        public ModelBuilder Build { get; }

        public Models.Field Instance { get; }

        public FieldBuilder Default(string value)
        {
            Instance.Default = value;
            return this;
        }

        public FieldBuilder DataType(DataTypes value)
        {
            Instance.DataType = value;
            return this;
        }

        public FieldBuilder BusinessKey(bool value)
        {
            Instance.BusinessKey = value;
            return this;
        }

        public FieldBuilder Nullable(bool value)
        {
            Instance.Nullable = value;
            return this;
        }

        public FieldBuilder MaxLength(int value)
        {
            if (Instance.MinLength.HasValue && value <= Instance.MinLength.Value)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"MaxLength must be greater than {Instance.MinLength.Value}");
            }

            Instance.MaxLength = value;
            return this;
        }

        public FieldBuilder MinLength(int value)
        {
            if (Instance.MaxLength.HasValue && value >= Instance.MaxLength.Value)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"MinLength must be less than {Instance.MaxLength.Value}");
            }

            Instance.MinLength = value;
            return this;
        }
    }
}
