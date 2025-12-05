namespace JJs.UnitsManagement.Sdk.Unit;

/// <summary>
/// Validator for read unit requests
/// </summary>
public class ReadUnitValidator : AbstractValidator<ReadUnitRequest>
{
    /// <summary>
    /// Initializes a new instance of the ReadUnitValidator class
    /// </summary>
    public ReadUnitValidator()
    {
        RuleFor(x => x.UnitId)
            .NotEmpty().WithMessage("Unit ID is required");
    }
}
