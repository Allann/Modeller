using System.Text;

namespace Modeller.Generator;

/// <summary>
/// Generates C# record classes from domain entities
/// </summary>
public sealed class CSharpRecordGenerator : ICodeGenerator
{
    public string Name => "CSharpRecords";

    public IEnumerable<GeneratedFile> Generate(Domain.Domain domain)
    {
        foreach (var entity in domain.Entities)
        {
            yield return GenerateEntity(entity, domain);
        }

        foreach (var enumeration in domain.Enums)
        {
            yield return GenerateEnum(enumeration);
        }
    }

    private static GeneratedFile GenerateEntity(Domain.Entity entity, Domain.Domain domain)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("namespace Generated;");
        sb.AppendLine();
        
        if (entity.Description is not null)
        {
            sb.AppendLine("/// <summary>");
            sb.AppendLine($"/// {entity.Description}");
            sb.AppendLine("/// </summary>");
        }

        sb.Append($"public sealed record {entity.Name}(");
        
        var parameters = new List<string>();
        
        // Add key fields first if present
        if (entity.Key?.Fields.Count > 0)
        {
            foreach (var field in entity.Key.Fields)
            {
                var csharpType = MapToCSharpType(field.TypeName, true);
                parameters.Add($"{csharpType} {field.Name}");
            }
        }
        
        // Add attributes
        foreach (var attr in entity.Attributes)
        {
            var csharpType = MapToCSharpType(attr.DataType.TypeName, attr.IsRequired);
            var defaultValue = attr.DefaultValue is not null ? $" = {FormatDefault(attr.DefaultValue, attr.DataType.TypeName)}" : "";
            
            // Make optional attributes nullable with default null
            if (!attr.IsRequired && string.IsNullOrEmpty(defaultValue))
            {
                defaultValue = " = null";
            }
            
            parameters.Add($"{csharpType} {attr.Name}{defaultValue}");
        }
        
        sb.Append(string.Join(", ", parameters));
        sb.AppendLine(");");
        
        return new GeneratedFile
        {
            Path = $"{entity.Name}.cs",
            Content = sb.ToString()
        };
    }

    private static GeneratedFile GenerateEnum(Domain.Enumeration enumeration)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("namespace Generated;");
        sb.AppendLine();
        
        if (enumeration.Description is not null)
        {
            sb.AppendLine("/// <summary>");
            sb.AppendLine($"/// {enumeration.Description}");
            sb.AppendLine("/// </summary>");
        }
        
        sb.AppendLine($"public enum {enumeration.Name}");
        sb.AppendLine("{");
        
        foreach (var value in enumeration.Values)
        {
            if (value.Description is not null)
            {
                sb.AppendLine($"    /// <summary>{value.Description}</summary>");
            }
            
            if (value.Value.HasValue)
            {
                sb.AppendLine($"    {value.Name} = {value.Value},");
            }
            else
            {
                sb.AppendLine($"    {value.Name},");
            }
        }
        
        sb.AppendLine("}");
        
        return new GeneratedFile
        {
            Path = $"{enumeration.Name}.cs",
            Content = sb.ToString()
        };
    }

    private static string MapToCSharpType(string dslType, bool required)
    {
        var nullableSuffix = required ? "" : "?";
        
        return dslType.ToLowerInvariant() switch
        {
            "text" => $"string{nullableSuffix}",
            "name" => $"string{nullableSuffix}",
            "guid" => $"Guid{nullableSuffix}",
            "boolean" => $"bool{nullableSuffix}",
            "integer" => $"int{nullableSuffix}",
            "decimal" => $"decimal{nullableSuffix}",
            "date" => $"DateOnly{nullableSuffix}",
            "datetime" => $"DateTime{nullableSuffix}",
            "time" => $"TimeOnly{nullableSuffix}",
            _ => $"string{nullableSuffix}" // Default to string for unknown types
        };
    }

    private static string FormatDefault(string value, string typeName)
    {
        return typeName.ToLowerInvariant() switch
        {
            "boolean" => value.ToLowerInvariant(),
            "text" or "name" => $"\"{value}\"",
            _ => value
        };
    }
}

