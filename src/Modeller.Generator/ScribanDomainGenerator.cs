namespace Modeller.Generator;

/// <summary>
/// Generates domain classes using Scriban templates
/// </summary>
public sealed class ScribanDomainGenerator : ICodeGenerator
{
    private readonly string _namespace;
    private readonly TemplateEngine _engine;

    public ScribanDomainGenerator(string @namespace, string templatesPath)
    {
        _namespace = @namespace;
        _engine = new TemplateEngine(templatesPath);
    }

    public string Name => "ScribanDomain";

    public IEnumerable<GeneratedFile> Generate(Domain.Domain domain)
    {
        // Generate entity records
        foreach (var entity in domain.Entities)
        {
            yield return GenerateFromTemplate(
                "csharp/domain/entity.scriban",
                $"{entity.Name}.cs",
                new { Namespace = _namespace, Entity = entity });
        }

        // Generate enumeration records
        foreach (var enumeration in domain.Enums)
        {
            yield return GenerateFromTemplate(
                "csharp/domain/enumeration.scriban",
                $"{enumeration.Name}.cs",
                new { Namespace = _namespace, Enumeration = enumeration });
        }

        // Generate command records
        foreach (var command in domain.Commands)
        {
            yield return GenerateFromTemplate(
                "csharp/domain/command.scriban",
                $"{command.Name}.cs",
                new { Namespace = _namespace, Command = command });
        }

        // Generate query records
        foreach (var query in domain.Queries)
        {
            yield return GenerateFromTemplate(
                "csharp/domain/query.scriban",
                $"{query.Name}.cs",
                new { Namespace = _namespace, Query = query });
        }
    }

    private GeneratedFile GenerateFromTemplate(string templatePath, string outputPath, object model)
    {
        var content = _engine.Render(templatePath, model);
        return new GeneratedFile { Path = outputPath, Content = content };
    }
}

