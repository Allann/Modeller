namespace JJs.UnitsManagement.Sdk.Unit;

/// <summary>
/// Validator for delete unit requests
/// </summary>
public class DeleteUnitValidator : AbstractValidator<DeleteUnitRequest>
{
    /// <summary>
    /// Initializes a new instance of the DeleteUnitValidator class
    /// </summary>
    public DeleteUnitValidator()
    {
        RuleFor(x => x.UnitId)
            .NotEmpty().WithMessage("Unit ID is required");
    }
}
