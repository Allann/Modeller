using Newtonsoft.Json;
using System.ComponentModel;

namespace Hy.Modeller.Models
{
    public class Field
    {
        private Field()
        {
        }

        public Field(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new System.ArgumentException("Must include a field name", nameof(name));
            }
            Name = new Name(name);
        }

        public Name Name { get; set; } = new Name("");

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Default { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxLength { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MinLength { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Decimals { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [DefaultValue(DataTypes.String)]
        public DataTypes DataType { get; set; } = DataTypes.String;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DataTypeTypeName { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [DefaultValue(true)]
        public bool Nullable { get; set; } = true;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [DefaultValue(false)]
        public bool BusinessKey { get; set; }
    }
}