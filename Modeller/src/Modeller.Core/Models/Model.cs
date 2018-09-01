using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace Modeller.Models
{
    public class Model
    {
        public Name Name { get; set; } = new Name("");

        public Key Key { get; } = new Key();

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [DefaultValue(true)]
        public bool HasAudit { get; set; } = true;

        public List<Field> Fields { get; } = new List<Field>();

        public List<Relationship> Relationships { get; } = new List<Relationship>();
    }
}
