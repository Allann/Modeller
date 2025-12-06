using Modeller.Generator.Configuration;
using Modeller.Generator.Templates;

namespace Modeller.Generator.Generation;

/// <summary>
/// Plans file generation based on profile, templates, and domain
/// </summary>
public sealed class GenerationPlanner
{
    private readonly TemplateDiscovery _discovery;
    private readonly VariableMerger _variableMerger;

    public GenerationPlanner(string templatesPath)
    {
        _discovery = new TemplateDiscovery(templatesPath);
        _variableMerger = new VariableMerger();
    }

    /// <summary>
    /// Create a generation plan for a layer
    /// </summary>
    public GenerationPlan CreatePlan(
        Domain.Domain domain,
        LayerConfig layerConfig,
        ProfileConfig profile,
        ProjectConfig projectConfig,
        string outputRoot,
        Dictionary<string, object>? cliOverrides = null)
    {
        // Merge variables for this layer
        var variables = _variableMerger.MergeVariables(
            projectConfig, profile, layerConfig.Name, cliOverrides);

        // Find the template for this layer
        var packs = _discovery.DiscoverPacks();
        var pack = packs.FirstOrDefault(p =>
            p.Name.Equals(profile.Pack, StringComparison.OrdinalIgnoreCase) ||
            p.Path.EndsWith(profile.Pack.Replace('/', Path.DirectorySeparatorChar)));

        if (pack == null)
            throw new InvalidOperationException($"Template pack not found: {profile.Pack}");

        var template = pack.Templates.FirstOrDefault(t =>
            t.Name.Equals(layerConfig.Template, StringComparison.OrdinalIgnoreCase));

        if (template == null)
            throw new InvalidOperationException($"Template not found: {layerConfig.Template} in pack {profile.Pack}");

        // Calculate output folder
        var layerOutput = _variableMerger.ResolvePattern(layerConfig.Output, variables);
        var layerOutputPath = Path.Combine(outputRoot, layerOutput);

        // Plan all files from the template manifest
        var plannedFiles = new List<PlannedFile>();

        if (template.Manifest != null)
        {
            foreach (var entry in template.Manifest.Generates)
            {
                plannedFiles.AddRange(PlanFilesForEntry(
                    entry, domain, template.Path, layerOutputPath, variables, profile));
            }
        }

        return new GenerationPlan
        {
            Domain = domain,
            Files = plannedFiles,
            LayerName = layerConfig.Name,
            OutputRoot = layerOutputPath
        };
    }

    private IEnumerable<PlannedFile> PlanFilesForEntry(
        GenerationEntry entry,
        Domain.Domain domain,
        string templateFolder,
        string outputFolder,
        Dictionary<string, object> variables,
        ProfileConfig profile)
    {
        var templatePath = Path.Combine(templateFolder, entry.Template);

        return entry.Per.ToLowerInvariant() switch
        {
            "domain" => PlanDomainFile(entry, domain, templatePath, outputFolder, variables),
            "entity" => PlanEntityFiles(entry, domain, templatePath, outputFolder, variables, profile),
            "enum" => PlanEnumFiles(entry, domain, templatePath, outputFolder, variables, profile),
            "command" => PlanCommandFiles(entry, domain, templatePath, outputFolder, variables, profile),
            "query" => PlanQueryFiles(entry, domain, templatePath, outputFolder, variables, profile),
            _ => []
        };
    }

    private IEnumerable<PlannedFile> PlanDomainFile(
        GenerationEntry entry, Domain.Domain domain,
        string templatePath, string outputFolder, Dictionary<string, object> variables)
    {
        var context = _variableMerger.CreateTemplateContext(variables, domain: domain);
        var relativePath = ResolveOutputPath(entry.Output, context);
        var fullPath = Path.Combine(outputFolder, relativePath);

        yield return new PlannedFile
        {
            OutputPath = fullPath,
            RelativePath = relativePath,
            TemplatePath = templatePath,
            Context = context,
            Description = entry.Description,
            ElementType = "domain",
            ElementName = domain.Name
        };
    }

    private IEnumerable<PlannedFile> PlanEntityFiles(
        GenerationEntry entry, Domain.Domain domain,
        string templatePath, string outputFolder, Dictionary<string, object> variables,
        ProfileConfig profile)
    {
        foreach (var entity in domain.Entities)
        {
            if (!profile.Include.ShouldInclude("entities", entity.Name)) continue;
            if (profile.Exclude.ShouldExclude("entities", entity.Name)) continue;

            var context = _variableMerger.CreateTemplateContext(variables, entity: entity, domain: domain);
            context["entity"] = entity;
            var relativePath = ResolveOutputPath(entry.Output, context);
            var fullPath = Path.Combine(outputFolder, relativePath);

            yield return new PlannedFile
            {
                OutputPath = fullPath,
                RelativePath = relativePath,
                TemplatePath = templatePath,
                Context = context,
                Description = entry.Description,
                ElementType = "entity",
                ElementName = entity.Name
            };
        }
    }

    private IEnumerable<PlannedFile> PlanEnumFiles(
        GenerationEntry entry, Domain.Domain domain,
        string templatePath, string outputFolder, Dictionary<string, object> variables,
        ProfileConfig profile)
    {
        foreach (var enumeration in domain.Enums)
        {
            if (!profile.Include.ShouldInclude("enums", enumeration.Name)) continue;

            var context = _variableMerger.CreateTemplateContext(variables, enumeration: enumeration, domain: domain);
            context["enumeration"] = enumeration;
            var relativePath = ResolveOutputPath(entry.Output, context);
            var fullPath = Path.Combine(outputFolder, relativePath);

            yield return new PlannedFile
            {
                OutputPath = fullPath,
                RelativePath = relativePath,
                TemplatePath = templatePath,
                Context = context,
                Description = entry.Description,
                ElementType = "enum",
                ElementName = enumeration.Name
            };
        }
    }

    private IEnumerable<PlannedFile> PlanCommandFiles(
        GenerationEntry entry, Domain.Domain domain,
        string templatePath, string outputFolder, Dictionary<string, object> variables,
        ProfileConfig profile)
    {
        foreach (var command in domain.Commands)
        {
            if (!profile.Include.ShouldInclude("commands", command.Name)) continue;

            var context = _variableMerger.CreateTemplateContext(variables, command: command, domain: domain);
            context["command"] = command;
            var relativePath = ResolveOutputPath(entry.Output, context);
            var fullPath = Path.Combine(outputFolder, relativePath);

            yield return new PlannedFile
            {
                OutputPath = fullPath,
                RelativePath = relativePath,
                TemplatePath = templatePath,
                Context = context,
                Description = entry.Description,
                ElementType = "command",
                ElementName = command.Name
            };
        }
    }

    private IEnumerable<PlannedFile> PlanQueryFiles(
        GenerationEntry entry, Domain.Domain domain,
        string templatePath, string outputFolder, Dictionary<string, object> variables,
        ProfileConfig profile)
    {
        foreach (var query in domain.Queries)
        {
            if (!profile.Include.ShouldInclude("queries", query.Name)) continue;

            var context = _variableMerger.CreateTemplateContext(variables, query: query, domain: domain);
            context["query"] = query;
            var relativePath = ResolveOutputPath(entry.Output, context);
            var fullPath = Path.Combine(outputFolder, relativePath);

            yield return new PlannedFile
            {
                OutputPath = fullPath,
                RelativePath = relativePath,
                TemplatePath = templatePath,
                Context = context,
                Description = entry.Description,
                ElementType = "query",
                ElementName = query.Name
            };
        }
    }

    /// <summary>
    /// Resolve output path pattern with context variables
    /// </summary>
    private static string ResolveOutputPath(string pattern, Dictionary<string, object> context)
    {
        var result = pattern;

        // Replace {variables.xxx} patterns
        if (context.TryGetValue("variables", out var variablesObj) && variablesObj is Dictionary<string, object> variables)
        {
            foreach (var kvp in variables)
            {
                result = result.Replace($"{{variables.{kvp.Key}}}", kvp.Value?.ToString() ?? "");
                result = result.Replace($"{{{{ variables.{kvp.Key} }}}}", kvp.Value?.ToString() ?? "");
            }
        }

        // Replace {{ entity.name | pascal_case }} style patterns
        if (context.TryGetValue("entity", out var entity) && entity is Domain.Entity e)
        {
            result = result.Replace("{{ entity.name | pascal_case }}", ToPascalCase(e.Name));
            result = result.Replace("{{entity.name | pascal_case}}", ToPascalCase(e.Name));
        }

        if (context.TryGetValue("enumeration", out var enumeration) && enumeration is Domain.Enumeration en)
        {
            result = result.Replace("{{ enumeration.name | pascal_case }}", ToPascalCase(en.Name));
            result = result.Replace("{{enumeration.name | pascal_case}}", ToPascalCase(en.Name));
        }

        if (context.TryGetValue("command", out var command) && command is Domain.Command cmd)
        {
            result = result.Replace("{{ command.name | pascal_case }}", ToPascalCase(cmd.Name));
            result = result.Replace("{{command.name | pascal_case}}", ToPascalCase(cmd.Name));
        }

        if (context.TryGetValue("query", out var query) && query is Domain.Query q)
        {
            result = result.Replace("{{ query.name | pascal_case }}", ToPascalCase(q.Name));
            result = result.Replace("{{query.name | pascal_case}}", ToPascalCase(q.Name));
        }

        if (context.TryGetValue("projection", out var projection) && projection is Domain.Entity p)
        {
            result = result.Replace("{{ projection.name | pascal_case }}", ToPascalCase(p.Name));
            result = result.Replace("{{projection.name | pascal_case}}", ToPascalCase(p.Name));
        }

        return result;
    }

    private static string ToPascalCase(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return char.ToUpperInvariant(value[0]) + value[1..];
    }
}

