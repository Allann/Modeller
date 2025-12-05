using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Modeller.Generator.Configuration;

/// <summary>
/// Loads manifest files from YAML.
/// </summary>
public sealed class ManifestLoader
{
    private readonly IDeserializer _deserializer;

    public ManifestLoader()
    {
        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
    }

    /// <summary>
    /// Loads a pack manifest from a YAML file.
    /// </summary>
    public PackManifest LoadPackManifest(string filePath)
    {
        var yaml = File.ReadAllText(filePath);
        return LoadPackManifestFromString(yaml);
    }

    /// <summary>
    /// Loads a pack manifest from a YAML string.
    /// </summary>
    public PackManifest LoadPackManifestFromString(string yaml)
    {
        return _deserializer.Deserialize<PackManifest>(yaml);
    }

    /// <summary>
    /// Loads a template manifest from a YAML file.
    /// </summary>
    public TemplateManifest LoadTemplateManifest(string filePath)
    {
        var yaml = File.ReadAllText(filePath);
        return LoadTemplateManifestFromString(yaml);
    }

    /// <summary>
    /// Loads a template manifest from a YAML string.
    /// </summary>
    public TemplateManifest LoadTemplateManifestFromString(string yaml)
    {
        return _deserializer.Deserialize<TemplateManifest>(yaml);
    }
}

