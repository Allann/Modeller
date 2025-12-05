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

