namespace JJs.UnitsManagement.Sdk.Unit;

/// <summary>
/// Validator for read all units requests
/// </summary>
public class ReadAllUnitsValidator : AbstractValidator<ReadAllUnitsRequest>
{
    /// <summary>
    /// Initializes a new instance of the ReadAllUnitsValidator class
    /// </summary>
    public ReadAllUnitsValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100).WithMessage("Search term must be 100 characters or less")
            .When(x => !string.IsNullOrEmpty(x.SearchTerm));

        RuleFor(x => x.Company)
            .MaximumLength(20).WithMessage("Company filter must be 20 characters or less")
            .When(x => !string.IsNullOrEmpty(x.Company));

        RuleFor(x => x.State)
            .MaximumLength(10).WithMessage("State filter must be 10 characters or less")
            .When(x => !string.IsNullOrEmpty(x.State));
    }
}
