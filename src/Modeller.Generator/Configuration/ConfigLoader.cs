using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Modeller.Generator.Configuration;

/// <summary>
/// Loads configuration from the .modeller folder
/// </summary>
public sealed class ConfigLoader
{
    private readonly IDeserializer _deserializer;

    public ConfigLoader()
    {
        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
    }

    /// <summary>
    /// Load project configuration from config.yaml
    /// </summary>
    public ProjectConfig LoadProjectConfig(string modellerFolder)
    {
        var configPath = Path.Combine(modellerFolder, "config.yaml");
        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Configuration file not found: {configPath}");
        }

        var yaml = File.ReadAllText(configPath);
        return LoadProjectConfigFromString(yaml);
    }

    /// <summary>
    /// Load project configuration from a YAML string
    /// </summary>
    public ProjectConfig LoadProjectConfigFromString(string yaml)
    {
        return _deserializer.Deserialize<ProjectConfig>(yaml);
    }

    /// <summary>
    /// Load a profile configuration from the profiles folder
    /// </summary>
    public ProfileConfig LoadProfile(string modellerFolder, string profileName)
    {
        var profilePath = Path.Combine(modellerFolder, "profiles", $"{profileName}.yaml");
        if (!File.Exists(profilePath))
        {
            throw new FileNotFoundException($"Profile not found: {profilePath}");
        }

        var yaml = File.ReadAllText(profilePath);
        return LoadProfileFromString(yaml);
    }

    /// <summary>
    /// Load profile configuration from a YAML string
    /// </summary>
    public ProfileConfig LoadProfileFromString(string yaml)
    {
        return _deserializer.Deserialize<ProfileConfig>(yaml);
    }

    /// <summary>
    /// List available profiles in the profiles folder
    /// </summary>
    public IEnumerable<string?> ListProfiles(string modellerFolder)
    {
        var profilesFolder = Path.Combine(modellerFolder, "profiles");
        if (!Directory.Exists(profilesFolder))
        {
            return [];
        }

        return Directory.GetFiles(profilesFolder, "*.yaml")
            .Select(Path.GetFileNameWithoutExtension);
    }

    /// <summary>
    /// Load the complete configuration including project config and default profile
    /// </summary>
    public LoadedConfiguration LoadConfiguration(string projectRoot)
    {
        var modellerFolder = Path.Combine(projectRoot, ".modeller");
        if (!Directory.Exists(modellerFolder))
        {
            throw new DirectoryNotFoundException($".modeller folder not found in: {projectRoot}");
        }

        var projectConfig = LoadProjectConfig(modellerFolder);
        var defaultProfile = LoadProfile(modellerFolder, projectConfig.DefaultProfile);

        return new(
            ProjectRoot: projectRoot,
            ModellerFolder: modellerFolder,
            ProjectConfig: projectConfig,
            DefaultProfile: defaultProfile);
    }

    /// <summary>
    /// Check if a .modeller folder exists in the given project root
    /// </summary>
    public bool HasConfiguration(string projectRoot)
    {
        var modellerFolder = Path.Combine(projectRoot, ".modeller");
        var configPath = Path.Combine(modellerFolder, "config.yaml");
        return File.Exists(configPath);
    }
}

/// <summary>
/// Represents a fully loaded configuration
/// </summary>
public sealed record LoadedConfiguration(
    string ProjectRoot,
    string ModellerFolder,
    ProjectConfig ProjectConfig,
    ProfileConfig DefaultProfile)
{
    /// <summary>
    /// Get the resolved domain folder path
    /// </summary>
    public string GetDomainPath()
    {
        var domain = ProjectConfig.Domain;
        if (Path.IsPathRooted(domain))
            return domain;

        return Path.GetFullPath(Path.Combine(ProjectRoot, domain));
    }

    /// <summary>
    /// Get the resolved output root path
    /// </summary>
    public string GetOutputRoot()
    {
        var root = ProjectConfig.Output.Root;
        if (Path.IsPathRooted(root))
            return root;

        return Path.GetFullPath(Path.Combine(ProjectRoot, root));
    }

    /// <summary>
    /// Get the templates folder path.
    /// Uses local .modeller/templates if it exists, otherwise falls back to template_source.
    /// </summary>
    public string GetTemplatesPath()
    {
        var localTemplates = Path.Combine(ModellerFolder, "templates");
        if (Directory.Exists(localTemplates))
            return localTemplates;

        // Fall back to template_source
        var source = ProjectConfig.TemplateSource;
        if (string.IsNullOrEmpty(source))
            return localTemplates;

        // Handle file:// prefix
        if (source.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
            source = source[7..];

        // Resolve relative paths
        if (!Path.IsPathRooted(source))
            return Path.GetFullPath(Path.Combine(ProjectRoot, source));

        return source;
    }
}

