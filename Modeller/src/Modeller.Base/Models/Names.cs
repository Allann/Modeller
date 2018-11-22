namespace Hy.Modeller.Models
{
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

        public override string ToString() => Value;
    }
}