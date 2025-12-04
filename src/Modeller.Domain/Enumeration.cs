namespace Modeller.Domain;

/// <summary>
/// Represents an enumeration type
/// </summary>
/// <param name="Name">The enum name</param>
/// <param name="Description">Optional description</param>
/// <param name="Values">The enum values</param>
public sealed record Enumeration(
    string Name,
    string? Description = null,
    IReadOnlyList<EnumValue>? Values = null)
{
    public IReadOnlyList<EnumValue> Values { get; init; } = Values ?? [];
}

/// <summary>
/// Represents a single value in an enumeration
/// </summary>
/// <param name="Name">The value name</param>
/// <param name="Description">Optional description</param>
/// <param name="Value">Optional explicit value</param>
public sealed record EnumValue(
    string Name,
    string? Description = null,
    int? Value = null);

