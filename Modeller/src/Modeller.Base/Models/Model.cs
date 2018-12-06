using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Hy.Modeller.Models
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

        public bool IsValid()
        {
            _errors.Clear();

            if (Key.Fields.Count == 0)
                _errors.Add($"Model {Name} must have a key");
                
            if (Fields.Count(f => f.BusinessKey == true) > 1)
                _errors.Add($"Model {Name} can only have at most, one business key");

            if (HasAudit && Fields.Count(f => f.Name.Singular.Value == "Created" || f.Name.Singular.Value == "CreatedBy" || f.Name.Singular.Value == "Modified" || f.Name.Singular.Value == "ModifiedBy") > 0)
                _errors.Add($"Model {Name} Audit fields shouldn't be added when HasAudit is true");

            var duplicates = Fields.GroupBy(f => f.Name).Where(g => g.Count() > 1).Select(f => f.Key);
            if (duplicates != null && duplicates.Count() > 0)
                _errors.Add($"Model {Name} shouldn't have duplicate field names ({string.Join(", ", duplicates)})");

            foreach(var f in Fields)
            {
                if(!f.IsValid()) _errors.AddRange(f.Errors);
            }
                       
            return !_errors.Any();
        }

        private List<string> _errors = new List<string>();

        [JsonIgnore]
        public IEnumerable<string> Errors => _errors.AsReadOnly();

    }
}
