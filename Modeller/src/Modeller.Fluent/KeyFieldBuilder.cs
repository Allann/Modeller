using Hy.Modeller.Models;
using System;
using System.ComponentModel;

namespace Hy.Modeller.Fluent
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class KeyFieldBuilder 
    {
        public KeyFieldBuilder(KeyBuilder keyBuilder, Models.Field field)
        {
            Build = keyBuilder ?? throw new ArgumentNullException(nameof(keyBuilder));
            Instance = field ?? throw new ArgumentNullException(nameof(field));
        }

        public KeyBuilder Build { get; }

        public Models.Field Instance { get; }

        public KeyFieldBuilder Default(string value)
        {
            Instance.Default = value;
            return this;
        }

        public KeyFieldBuilder DataType(DataTypes value)
        {
            Instance.DataType = value;
            return this;
        }

        public KeyFieldBuilder BusinessKey(bool value)
        {
            Instance.BusinessKey = value;
            return this;
        }

        public KeyFieldBuilder Nullable(bool value)
        {
            Instance.Nullable = value;
            return this;
        }

        public KeyFieldBuilder MaxLength(int value)
        {
            if (Instance.MinLength.HasValue && value <= Instance.MinLength.Value)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"MaxLength must be greater than {Instance.MinLength.Value}");
            }

            Instance.MaxLength = value;
            return this;
        }

        public KeyFieldBuilder MinLength(int value)
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
