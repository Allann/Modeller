namespace Hy.Modeller.Interfaces
{
    public interface ISettings
    {
        bool SupportRegen { get; set; }

        string GetPackageVersion(string name);

        GeneratorContext Context { get; }
    }
}