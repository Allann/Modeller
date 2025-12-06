using YamlDotNet.Serialization;

namespace Modeller.Generator.Configuration;

/// <summary>
/// Model for .modeller/config.yaml - main project configuration
/// </summary>
public sealed class ProjectConfig
{
    /// <summary>
    /// Schema version for the configuration file
    /// </summary>
    [YamlMember(Alias = "version")]
    public int Version { get; set; } = 1;

    /// <summary>
    /// Path to domain definitions folder (relative to .modeller or absolute)
    /// </summary>
    [YamlMember(Alias = "domain")]
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// Source URL/path where templates were copied from
    /// </summary>
    [YamlMember(Alias = "template_source")]
    public string TemplateSource { get; set; } = string.Empty;

    /// <summary>
    /// Global variables available to all templates
    /// </summary>
    [YamlMember(Alias = "variables")]
    public Dictionary<string, object> Variables { get; set; } = [];

    /// <summary>
    /// Output configuration
    /// </summary>
    [YamlMember(Alias = "output")]
    public OutputConfig Output { get; set; } = new();

    /// <summary>
    /// Default profile to use when none specified
    /// </summary>
    [YamlMember(Alias = "default_profile")]
    public string DefaultProfile { get; set; } = "default";

    /// <summary>
    /// File handling configuration
    /// </summary>
    [YamlMember(Alias = "files")]
    public FileConfig Files { get; set; } = new();
}

/// <summary>
/// Output configuration settings
/// </summary>
public sealed class OutputConfig
{
    /// <summary>
    /// Root output directory (relative to project root or absolute)
    /// </summary>
    [YamlMember(Alias = "root")]
    public string Root { get; set; } = "./src";

    /// <summary>
    /// Pattern for project folder names
    /// </summary>
    [YamlMember(Alias = "project_pattern")]
    public string ProjectPattern { get; set; } = "{variables.company}.{variables.product}.{layer}";
}

/// <summary>
/// File handling configuration
/// </summary>
public sealed class FileConfig
{
    /// <summary>
    /// Suffix for generated files that can be overwritten (e.g., ".g")
    /// </summary>
    [YamlMember(Alias = "generated_suffix")]
    public string GeneratedSuffix { get; set; } = ".g";

    /// <summary>
    /// Line ending style: lf, crlf, or auto
    /// </summary>
    [YamlMember(Alias = "line_ending")]
    public string LineEnding { get; set; } = "auto";

    /// <summary>
    /// File encoding
    /// </summary>
    [YamlMember(Alias = "encoding")]
    public string Encoding { get; set; } = "utf-8";
}

