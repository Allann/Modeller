using Modeller.Generator.Configuration;

namespace Modeller.Generator.Templates;

/// <summary>
/// Discovers and loads template packs and their manifests from a templates folder.
/// </summary>
/// <remarks>
/// Expected folder structure:
///   templates/
///     _snippets/
///       csharp/
///         header.scriban
///         property.scriban
///     csharp/
///       clean-architecture/
///         pack.yaml
///         infrastructure/
///           template.yaml
///           entity.scriban
///         domain/
///           template.yaml
///           entity.scriban
/// </remarks>
public sealed class TemplateDiscovery
{
    private readonly string _templatesRoot;
    private readonly ManifestLoader _manifestLoader;

    /// <summary>
    /// Creates a new TemplateDiscovery instance.
    /// </summary>
    /// <param name="templatesRoot">Root path of the templates folder</param>
    public TemplateDiscovery(string templatesRoot)
    {
        _templatesRoot = templatesRoot ?? throw new ArgumentNullException(nameof(templatesRoot));
        _manifestLoader = new ManifestLoader();
    }

    /// <summary>
    /// Discovers all template packs in the templates folder.
    /// </summary>
    /// <returns>Collection of discovered packs with their manifests</returns>
    public IEnumerable<DiscoveredPack> DiscoverPacks()
    {
        if (!Directory.Exists(_templatesRoot))
        {
            yield break;
        }

        // Look for pack.yaml files in language/pack-name folders
        foreach (var languageDir in Directory.GetDirectories(_templatesRoot))
        {
            var languageName = Path.GetFileName(languageDir);
            
            // Skip _snippets folder
            if (languageName.StartsWith("_"))
            {
                continue;
            }

            foreach (var packDir in Directory.GetDirectories(languageDir))
            {
                var packYamlPath = Path.Combine(packDir, "pack.yaml");
                if (File.Exists(packYamlPath))
                {
                    var manifest = _manifestLoader.LoadPackManifest(packYamlPath);
                    var templates = DiscoverTemplates(packDir).ToList();
                    
                    yield return new DiscoveredPack
                    {
                        Path = packDir,
                        Language = languageName,
                        Manifest = manifest,
                        Templates = templates
                    };
                }
            }
        }
    }

    /// <summary>
    /// Discovers all templates within a pack folder.
    /// </summary>
    private IEnumerable<DiscoveredTemplate> DiscoverTemplates(string packDir)
    {
        // Look for template.yaml files in subdirectories
        foreach (var templateDir in Directory.GetDirectories(packDir))
        {
            var templateYamlPath = Path.Combine(templateDir, "template.yaml");
            if (File.Exists(templateYamlPath))
            {
                var manifest = _manifestLoader.LoadTemplateManifest(templateYamlPath);
                var templateName = Path.GetFileName(templateDir);
                
                yield return new DiscoveredTemplate
                {
                    Path = templateDir,
                    Name = templateName,
                    Manifest = manifest
                };
            }
        }
    }

    /// <summary>
    /// Gets a specific pack by language and name.
    /// </summary>
    public DiscoveredPack? GetPack(string language, string packName)
    {
        var packDir = Path.Combine(_templatesRoot, language, packName);
        var packYamlPath = Path.Combine(packDir, "pack.yaml");
        
        if (!File.Exists(packYamlPath))
        {
            return null;
        }

        var manifest = _manifestLoader.LoadPackManifest(packYamlPath);
        var templates = DiscoverTemplates(packDir).ToList();
        
        return new DiscoveredPack
        {
            Path = packDir,
            Language = language,
            Manifest = manifest,
            Templates = templates
        };
    }

    /// <summary>
    /// Gets a specific template within a pack.
    /// </summary>
    public DiscoveredTemplate? GetTemplate(string language, string packName, string templateName)
    {
        var templateDir = Path.Combine(_templatesRoot, language, packName, templateName);
        var templateYamlPath = Path.Combine(templateDir, "template.yaml");
        
        if (!File.Exists(templateYamlPath))
        {
            return null;
        }

        var manifest = _manifestLoader.LoadTemplateManifest(templateYamlPath);
        
        return new DiscoveredTemplate
        {
            Path = templateDir,
            Name = templateName,
            Manifest = manifest
        };
    }
}

