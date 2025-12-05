using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace Modeller.Generator.Templates;

/// <summary>
/// Loads Scriban snippets from the _snippets folder.
/// Implements ITemplateLoader to support include statements in templates.
/// </summary>
/// <remarks>
/// Snippets are organized by language:
///   _snippets/csharp/header.scriban
///   _snippets/csharp/property.scriban
///   _snippets/csharp/factory-methods.scriban
/// 
/// Templates can include snippets using:
///   {{ include '_snippets/csharp/header' }}
/// </remarks>
public sealed class SnippetLoader : ITemplateLoader
{
    private readonly string _templatesRoot;
    private readonly Dictionary<string, string> _cache = new();

    /// <summary>
    /// Creates a new SnippetLoader.
    /// </summary>
    /// <param name="templatesRoot">Root path of the templates folder (parent of _snippets)</param>
    public SnippetLoader(string templatesRoot)
    {
        _templatesRoot = templatesRoot ?? throw new ArgumentNullException(nameof(templatesRoot));
    }

    /// <summary>
    /// Gets the absolute path for a template include.
    /// </summary>
    /// <param name="context">The current template context</param>
    /// <param name="callerSpan">The source span of the include statement</param>
    /// <param name="templateName">The template name from the include statement</param>
    /// <returns>The absolute file path for the template</returns>
    public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
    {
        // Normalize the template name - remove leading slashes and ensure .scriban extension
        var normalizedName = templateName.TrimStart('/', '\\');
        
        if (!normalizedName.EndsWith(".scriban", StringComparison.OrdinalIgnoreCase))
        {
            normalizedName += ".scriban";
        }

        // Build the full path
        var fullPath = Path.GetFullPath(Path.Combine(_templatesRoot, normalizedName));
        
        // Security check: ensure the path is within the templates root
        var normalizedRoot = Path.GetFullPath(_templatesRoot);
        if (!fullPath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Template path '{templateName}' resolves outside the templates folder.");
        }

        return fullPath;
    }

    /// <summary>
    /// Loads the template content from the specified path.
    /// </summary>
    /// <param name="context">The current template context</param>
    /// <param name="callerSpan">The source span of the include statement</param>
    /// <param name="templatePath">The absolute path returned by GetPath</param>
    /// <returns>The template content</returns>
    public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
    {
        // Check cache first
        if (_cache.TryGetValue(templatePath, out var cached))
        {
            return cached;
        }

        // Load from disk
        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException(
                $"Template file not found: {templatePath}",
                templatePath);
        }

        var content = File.ReadAllText(templatePath);
        _cache[templatePath] = content;
        return content;
    }

    /// <summary>
    /// Asynchronously loads the template content from the specified path.
    /// </summary>
    /// <param name="context">The current template context</param>
    /// <param name="callerSpan">The source span of the include statement</param>
    /// <param name="templatePath">The absolute path returned by GetPath</param>
    /// <returns>The template content</returns>
    public async ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath)
    {
        // Check cache first
        if (_cache.TryGetValue(templatePath, out var cached))
        {
            return cached;
        }

        // Load from disk
        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException(
                $"Template file not found: {templatePath}",
                templatePath);
        }

        var content = await File.ReadAllTextAsync(templatePath);
        _cache[templatePath] = content;
        return content;
    }

    /// <summary>
    /// Clears the template cache.
    /// </summary>
    public void ClearCache()
    {
        _cache.Clear();
    }

    /// <summary>
    /// Gets the number of cached templates.
    /// </summary>
    public int CacheCount => _cache.Count;
}

