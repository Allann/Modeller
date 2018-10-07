using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Modeller.Models
{
    public class Model
    {
        public Name Name { get; set; } = new Name("");

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Schema { get; set; }

        public Key Key { get; } = new Key();

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [DefaultValue(true)]
        public bool HasAudit { get; set; } = true;

        public List<Field> Fields { get; } = new List<Field>();

        public List<Index> Indexes { get; } = new List<Index>();

        public List<Relationship> Relationships { get; } = new List<Relationship>();
    }
}
