using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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

        public bool IsValid()
        {
            _errors.Clear();

            if (Name == null || string.IsNullOrWhiteSpace(Name.ToString()))
            {
                _errors.Add("Field Name is missing");
            }
            if (Decimals.GetValueOrDefault(0) < 0)
            {
                _errors.Add($"Field {Name}.Decimals must be zero or greater");
            }
            if (MaxLength.GetValueOrDefault(0) < 0)
            {
                _errors.Add($"Field { Name}.MaxLength must be zero or greater");
            }
            if (MinLength.GetValueOrDefault(0) < 0)
            {
                _errors.Add($"Field {Name}.MinLength must be zero or greater");
            }
            return !_errors.Any();
        }

        private List<string> _errors = new List<string>();

        [JsonIgnore]
        public IEnumerable<string> Errors => _errors.AsReadOnly();
    }
}