using Humanizer;
using Newtonsoft.Json;
using System;

namespace Modeller.Models
{
    public class Name : IEquatable<Name>
    {
        public Name(string value)
        {
            SetName(value);
        }

        public void SetName(string name)
        {
            var value = string.IsNullOrWhiteSpace(name) ? string.Empty : name.Trim();
            if (value.Length == 0)
            {
                Plural = new Names(string.Empty, string.Empty, string.Empty, string.Empty);
                Singular = new Names(string.Empty, string.Empty, string.Empty, string.Empty);
                return;
            }

            var t = value.Humanize().Transform(To.TitleCase);
            var s = t.Singularize(false);
            var p = t.Pluralize(false);

            var ds = s.Dehumanize();
            var dp = p.Dehumanize();

            Plural = new Names(dp, dp.Camelize(), dp.Pascalize(), p);
            Singular = new Names(ds, ds.Camelize(), ds.Pascalize(), s);
        }

        public bool Equals(Name other) => other == null ? false : Singular.Value == other.Singular.Value;

        [JsonIgnore]
        public Names Singular { get; private set; }

        [JsonIgnore]
        public Names Plural { get; private set; }
    }

    public class Names
    {
        public Names(string value, string local, string stat, string display)
        {
            Value = value;
            LocalVariable = local;
            ModuleVariable = string.IsNullOrWhiteSpace(value) ? string.Empty : "_" + local;
            StaticVariable = stat;
            Display = display;
        }

        public string LocalVariable { get; }
        public string ModuleVariable { get; }
        public string StaticVariable { get; }
        public string Display { get; }
        public string Value { get; }
    }
}