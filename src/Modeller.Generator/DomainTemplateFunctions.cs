using Modeller.Domain;
using DomainAttribute = Modeller.Domain.Attribute;

namespace Modeller.Generator;

/// <summary>
/// Custom Scriban functions for domain code generation
/// </summary>
public static class DomainTemplateFunctions
{
    /// <summary>
    /// Converts a string to PascalCase
    /// </summary>
    public static string PascalCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return char.ToUpperInvariant(input[0]) + input[1..];
    }

    /// <summary>
    /// Converts a string to camelCase
    /// </summary>
    public static string CamelCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return char.ToLowerInvariant(input[0]) + input[1..];
    }

    /// <summary>
    /// Converts a string to lowercase
    /// </summary>
    public static string Downcase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return input.ToLowerInvariant();
    }

    /// <summary>
    /// Converts a string to uppercase
    /// </summary>
    public static string Upcase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return input.ToUpperInvariant();
    }

    /// <summary>
    /// Converts a string to snake_case
    /// </summary>
    public static string SnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var result = new System.Text.StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            var c = input[i];
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

    /// <summary>
    /// Converts a PascalCase or camelCase string to human-readable format
    /// </summary>
    public static string Humanize(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        var result = new System.Text.StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            var c = input[i];
            if (char.IsUpper(c) && i > 0)
            {
                result.Append(' ');
            }
            result.Append(i == 0 ? char.ToUpperInvariant(c) : c);
        }
        return result.ToString();
    }

    /// <summary>
    /// Pluralizes a word (simple English rules)
    /// </summary>
    public static string Pluralize(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        // Handle common irregular plurals
        var irregulars = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "person", "people" },
            { "child", "children" },
            { "man", "men" },
            { "woman", "women" },
            { "tooth", "teeth" },
            { "foot", "feet" },
            { "mouse", "mice" },
            { "goose", "geese" },
            { "ox", "oxen" },
            { "status", "statuses" },
            { "index", "indices" },
            { "matrix", "matrices" },
            { "vertex", "vertices" },
            { "axis", "axes" },
            { "crisis", "crises" },
            { "analysis", "analyses" },
            { "basis", "bases" },
            { "diagnosis", "diagnoses" },
            { "thesis", "theses" },
            { "hypothesis", "hypotheses" },
            { "parenthesis", "parentheses" },
            { "synopsis", "synopses" },
            { "datum", "data" },
            { "medium", "media" },
            { "criterion", "criteria" },
            { "phenomenon", "phenomena" },
            { "curriculum", "curricula" },
            { "memorandum", "memoranda" },
            { "appendix", "appendices" },
            { "quiz", "quizzes" }
        };

        if (irregulars.TryGetValue(input, out var irregular))
        {
            // Preserve original casing
            if (char.IsUpper(input[0]))
                return char.ToUpperInvariant(irregular[0]) + irregular[1..];
            return irregular;
        }

        // Words ending in 'y' preceded by a consonant
        if (input.EndsWith("y", StringComparison.OrdinalIgnoreCase) && input.Length > 1)
        {
            var beforeY = input[^2];
            if (!"aeiouAEIOU".Contains(beforeY))
            {
                return input[..^1] + "ies";
            }
        }

        // Words ending in 's', 'x', 'z', 'ch', 'sh'
        if (input.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
            input.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
            input.EndsWith("z", StringComparison.OrdinalIgnoreCase) ||
            input.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
            input.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
        {
            return input + "es";
        }

        // Words ending in 'f' or 'fe'
        if (input.EndsWith("fe", StringComparison.OrdinalIgnoreCase))
        {
            return input[..^2] + "ves";
        }
        if (input.EndsWith("f", StringComparison.OrdinalIgnoreCase))
        {
            return input[..^1] + "ves";
        }

        // Default: just add 's'
        return input + "s";
    }

    /// <summary>
    /// Singularizes a word (simple English rules)
    /// </summary>
    public static string Singularize(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        // Handle common irregular plurals (reverse lookup)
        var irregulars = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "people", "person" },
            { "children", "child" },
            { "men", "man" },
            { "women", "woman" },
            { "teeth", "tooth" },
            { "feet", "foot" },
            { "mice", "mouse" },
            { "geese", "goose" },
            { "oxen", "ox" },
            { "statuses", "status" },
            { "indices", "index" },
            { "matrices", "matrix" },
            { "vertices", "vertex" },
            { "axes", "axis" },
            { "crises", "crisis" },
            { "analyses", "analysis" },
            { "bases", "basis" },
            { "diagnoses", "diagnosis" },
            { "theses", "thesis" },
            { "hypotheses", "hypothesis" },
            { "parentheses", "parenthesis" },
            { "synopses", "synopsis" },
            { "data", "datum" },
            { "media", "medium" },
            { "criteria", "criterion" },
            { "phenomena", "phenomenon" },
            { "curricula", "curriculum" },
            { "memoranda", "memorandum" },
            { "appendices", "appendix" },
            { "quizzes", "quiz" }
        };

        if (irregulars.TryGetValue(input, out var irregular))
        {
            // Preserve original casing
            if (char.IsUpper(input[0]))
                return char.ToUpperInvariant(irregular[0]) + irregular[1..];
            return irregular;
        }

        // Words ending in 'ies'
        if (input.EndsWith("ies", StringComparison.OrdinalIgnoreCase) && input.Length > 3)
        {
            return input[..^3] + "y";
        }

        // Words ending in 'ves'
        if (input.EndsWith("ves", StringComparison.OrdinalIgnoreCase) && input.Length > 3)
        {
            return input[..^3] + "f";
        }

        // Words ending in 'es' (for s, x, z, ch, sh)
        if (input.EndsWith("es", StringComparison.OrdinalIgnoreCase) && input.Length > 2)
        {
            var stem = input[..^2];
            if (stem.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
                stem.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
                stem.EndsWith("z", StringComparison.OrdinalIgnoreCase) ||
                stem.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
                stem.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
            {
                return stem;
            }
        }

        // Default: remove trailing 's'
        if (input.EndsWith("s", StringComparison.OrdinalIgnoreCase) && input.Length > 1)
        {
            return input[..^1];
        }

        return input;
    }

    /// <summary>
    /// Converts a domain DataType to C# type string
    /// </summary>
    public static string ToCsharpType(DomainAttribute attr)
    {
        var baseType = MapTypeToCSharp(attr.DataType.TypeName);
        if (!attr.IsRequired && IsValueType(baseType))
            return baseType + "?";
        if (!attr.IsRequired && !IsValueType(baseType))
            return baseType + "?";
        return baseType;
    }

    /// <summary>
    /// Generates a C# parameter declaration for an attribute (legacy)
    /// </summary>
    public static string ToCsharpParam(DomainAttribute attr)
    {
        var type = ToCsharpType(attr);
        var name = CamelCase(attr.Name);

        if (!attr.IsRequired)
            return $"{type} {name} = null";
        if (attr.DefaultValue != null)
            return $"{type} {name} = {attr.DefaultValue}";
        return $"{type} {name}";
    }

    /// <summary>
    /// Generates a C# constructor parameter for an attribute
    /// </summary>
    public static string ToCsharpCtorParam(DomainAttribute attr)
    {
        var type = ToCsharpType(attr);
        var name = CamelCase(attr.Name);

        // In constructor, optional params have defaults
        if (!attr.IsRequired)
            return $"{type} {name} = null";
        if (attr.DefaultValue != null)
            return $"{type} {name} = {attr.DefaultValue}";
        return $"{type} {name}";
    }

    /// <summary>
    /// Generates a C# factory method parameter for an attribute
    /// </summary>
    public static string ToCsharpFactoryParam(DomainAttribute attr)
    {
        var type = ToCsharpType(attr);
        var name = CamelCase(attr.Name);

        if (!attr.IsRequired)
            return $"{type} {name} = null";
        if (attr.DefaultValue != null)
            return $"{type} {name} = {attr.DefaultValue}";
        return $"{type} {name}";
    }

    /// <summary>
    /// Gets the constructor assignment value for an attribute
    /// </summary>
    public static string ToCtorAssignment(DomainAttribute attr)
    {
        var name = CamelCase(attr.Name);
        var baseType = MapTypeToCSharp(attr.DataType.TypeName);

        // Collections get ?? [] treatment
        if (baseType.StartsWith("IReadOnlyList"))
            return $"{name} ?? []";

        return name;
    }

    /// <summary>
    /// Checks if an attribute needs validation in CreateValid
    /// </summary>
    public static bool NeedsValidation(DomainAttribute attr)
    {
        if (!attr.IsRequired) return false;

        var baseType = MapTypeToCSharp(attr.DataType.TypeName);
        // String types need null/whitespace validation
        return baseType == "string";
    }

    /// <summary>
    /// Gets the validation check expression for an attribute
    /// </summary>
    public static string ToValidationCheck(DomainAttribute attr)
    {
        var name = CamelCase(attr.Name);
        var baseType = MapTypeToCSharp(attr.DataType.TypeName);

        if (baseType == "string")
            return $"string.IsNullOrWhiteSpace({name})";

        return $"{name} == null";
    }

    /// <summary>
    /// Converts a relationship to C# type
    /// </summary>
    public static string ToCsharpRelType(Relationship rel)
    {
        return rel.Type switch
        {
            RelationshipType.HasMany => $"IReadOnlyList<{rel.TargetEntityName}>",
            RelationshipType.HasOne => $"{rel.TargetEntityName}?",
            RelationshipType.BelongsTo => $"{rel.TargetEntityName}?",
            _ => rel.TargetEntityName
        };
    }

    /// <summary>
    /// Generates a C# parameter declaration for a relationship
    /// </summary>
    public static string ToCsharpRelParam(Relationship rel)
    {
        var type = ToCsharpRelType(rel);
        var name = CamelCase(ToPropertyName(rel));
        
        return rel.Type switch
        {
            RelationshipType.HasMany => $"{type}? {name} = null",
            _ => $"{type} {name} = null"
        };
    }

    /// <summary>
    /// Gets the property name for a relationship (uses alias if present)
    /// </summary>
    public static string ToPropertyName(Relationship rel)
    {
        if (!string.IsNullOrEmpty(rel.Alias))
            return PascalCase(rel.Alias);
        
        return rel.Type == RelationshipType.HasMany 
            ? PascalCase(rel.TargetEntityName) + "s"  // Simple pluralization
            : PascalCase(rel.TargetEntityName);
    }

    /// <summary>
    /// Gets the argument name for a relationship
    /// </summary>
    public static string ToArgName(Relationship rel)
    {
        return CamelCase(ToPropertyName(rel));
    }

    /// <summary>
    /// Gets the tuple assignment value for a relationship
    /// </summary>
    public static string ToTupleValue(Relationship rel)
    {
        var name = CamelCase(ToPropertyName(rel));
        return rel.Type == RelationshipType.HasMany 
            ? $"{name} ?? []"
            : name;
    }

    /// <summary>
    /// Formats a default value for C# code output
    /// </summary>
    public static string ToCsharpDefaultValue(DomainAttribute attr)
    {
        if (attr.DefaultValue == null) return "";

        var baseType = MapTypeToCSharp(attr.DataType.TypeName);
        var value = attr.DefaultValue;

        // String types need quotes
        if (baseType == "string")
            return $"\"{value}\"";

        // Boolean values need lowercase
        if (baseType == "bool")
            return value.ToLowerInvariant();

        // Numeric types are used as-is
        return value;
    }

    private static string MapTypeToCSharp(string typeName) => typeName.ToLowerInvariant() switch
    {
        "text" or "string" or "name" => "string",
        "integer" or "int" => "int",
        "decimal" or "money" => "decimal",
        "boolean" or "bool" => "bool",
        "date" => "DateOnly",
        "datetime" => "DateTime",
        "guid" => "Guid",
        "long" => "long",
        "short" => "short",
        "byte" => "byte",
        "float" => "float",
        "double" => "double",
        _ => typeName
    };

    private static bool IsValueType(string csharpType) => csharpType switch
    {
        "int" or "long" or "short" or "byte" => true,
        "decimal" or "float" or "double" => true,
        "bool" => true,
        "DateOnly" or "DateTime" => true,
        "Guid" => true,
        _ => false
    };
}

