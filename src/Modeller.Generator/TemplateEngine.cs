using Scriban;
using Scriban.Runtime;

using Modeller.Generator.Templates;

namespace Modeller.Generator;

/// <summary>
/// Template engine that loads and renders Scriban templates.
/// Supports include statements for loading snippets from _snippets folder.
/// </summary>
public sealed class TemplateEngine
{
    private readonly string _templatesPath;
    private readonly ScriptObject _customFunctions;
    private readonly SnippetLoader _snippetLoader;

    public TemplateEngine(string templatesPath)
    {
        _templatesPath = templatesPath;
        _customFunctions = CreateCustomFunctions();
        _snippetLoader = new SnippetLoader(templatesPath);
    }

    /// <summary>
    /// Renders a template with the given model.
    /// Supports include statements like: {{ include '_snippets/csharp/header' }}
    /// </summary>
    public string Render(string templatePath, object model)
    {
        var fullPath = Path.Combine(_templatesPath, templatePath);
        var templateText = File.ReadAllText(fullPath);
        var template = Template.Parse(templateText, fullPath);

        if (template.HasErrors)
        {
            var errors = string.Join("\n", template.Messages.Select(m => m.ToString()));
            throw new InvalidOperationException($"Template parse errors in {templatePath}:\n{errors}");
        }

        var context = CreateContext(model);
        return template.Render(context);
    }

    /// <summary>
    /// Renders a template from string content with the given model.
    /// Supports include statements like: {{ include '_snippets/csharp/header' }}
    /// </summary>
    public string RenderFromString(string templateContent, object model)
    {
        var template = Template.Parse(templateContent);

        if (template.HasErrors)
        {
            var errors = string.Join("\n", template.Messages.Select(m => m.ToString()));
            throw new InvalidOperationException($"Template parse errors:\n{errors}");
        }

        var context = CreateContext(model);
        return template.Render(context);
    }

    /// <summary>
    /// Creates a template context with custom functions, model, and snippet loader.
    /// </summary>
    private TemplateContext CreateContext(object model)
    {
        var context = new TemplateContext
        {
            TemplateLoader = _snippetLoader
        };

        context.PushGlobal(_customFunctions);

        var scriptObject = new ScriptObject();
        scriptObject.Import(model, renamer: member => ToSnakeCase(member.Name));
        context.PushGlobal(scriptObject);

        return context;
    }

    /// <summary>
    /// Clears the snippet cache. Useful when templates have been modified.
    /// </summary>
    public void ClearCache()
    {
        _snippetLoader.ClearCache();
    }

    private static ScriptObject CreateCustomFunctions()
    {
        var functions = new ScriptObject();
        functions.Import(typeof(DomainTemplateFunctions));
        return functions;
    }

    private static string ToSnakeCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;

        var result = new System.Text.StringBuilder();
        for (int i = 0; i < name.Length; i++)
        {
            var c = name[i];
            if (char.IsUpper(c))
            {
                if (i > 0) result.Append('_');
                result.Append(char.ToLowerInvariant(c));
            }
            else
            {
                result.Append(c);
            }
        }
        return result.ToString();
    }
}

