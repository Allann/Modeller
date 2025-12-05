namespace JJs.UnitsManagement.Sdk.Unit;

/// <summary>
/// Request to delete a unit
/// </summary>
public record DeleteUnitRequest
{
    /// <summary>
    /// The unique identifier for the unit
    /// </summary>
    [Required]
    public required Guid UnitId { get; init; }
}
