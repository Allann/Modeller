using Newtonsoft.Json;
using System.ComponentModel;

namespace Modeller.Models
{
    public class IndexField
    {

        public IndexField(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new System.ArgumentException("Index field name can not be empty.", nameof(name));
            Name = new Name(name);
        }

        public Name Name { get; set; } = new Name("");

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        [DefaultValue(ListSortDirection.Ascending)]
        public ListSortDirection Sort { get; set; } = ListSortDirection.Ascending;
    }
}
