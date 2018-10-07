using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace Modeller.Models
{
    public class Index
    {
        private Index() { }

        public Index(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new System.ArgumentException("Must include an index name", nameof(name));
            }
            Name = new Name(name);
        }

        public Name Name { get; set; } = new Name("");

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [DefaultValue(false)]
        public bool IsUnique { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [DefaultValue(false)]
        public bool IsClustered { get; set; }

        public List<IndexField> Fields { get; } = new List<IndexField>();
    }
}
