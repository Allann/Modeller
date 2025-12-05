namespace JJs.UnitsManagement.Sdk.Unit;

/// <summary>
/// Request to read a single unit
/// </summary>
public record ReadUnitRequest
{
    /// <summary>
    /// The unique identifier for the unit
    /// </summary>
    [Required]
    public required Guid UnitId { get; init; }
}
