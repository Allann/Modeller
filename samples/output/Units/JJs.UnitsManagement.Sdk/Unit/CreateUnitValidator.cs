namespace JJs.UnitsManagement.Sdk.Unit;

/// <summary>
/// Validator for create unit requests
/// </summary>
public class CreateUnitValidator : AbstractValidator<CreateUnitRequest>
{
    /// <summary>
    /// Initializes a new instance of the CreateUnitValidator class
    /// </summary>
    public CreateUnitValidator()
    {
        RuleFor(x => x.TruckNumber)
            .NotEmpty().WithMessage("Truck number is required")
            .MaximumLength(50).WithMessage("Truck number must be 50 characters or less");

        RuleFor(x => x.RegistrationNumber)
            .MaximumLength(20).WithMessage("Registration number must be 20 characters or less")
            .When(x => !string.IsNullOrEmpty(x.RegistrationNumber));

        RuleFor(x => x.Description)
            .MaximumLength(200).WithMessage("Description must be 200 characters or less")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Make)
            .MaximumLength(50).WithMessage("Make must be 50 characters or less")
            .When(x => !string.IsNullOrEmpty(x.Make));

        RuleFor(x => x.Model)
            .MaximumLength(100).WithMessage("Model must be 100 characters or less")
            .When(x => !string.IsNullOrEmpty(x.Model));

        RuleFor(x => x.EuroType)
            .MaximumLength(20).WithMessage("Euro type must be 20 characters or less")
            .When(x => !string.IsNullOrEmpty(x.EuroType));

        RuleFor(x => x.EngineNumber)
            .MaximumLength(50).WithMessage("Engine number must be 50 characters or less")
            .When(x => !string.IsNullOrEmpty(x.EngineNumber));

        RuleFor(x => x.ChassisNumber)
            .MaximumLength(50).WithMessage("Chassis number must be 50 characters or less")
            .When(x => !string.IsNullOrEmpty(x.ChassisNumber));

        RuleFor(x => x.State)
            .MaximumLength(10).WithMessage("State must be 10 characters or less")
            .When(x => !string.IsNullOrEmpty(x.State));

        RuleFor(x => x.Company)
            .MaximumLength(20).WithMessage("Company must be 20 characters or less")
            .When(x => !string.IsNullOrEmpty(x.Company));

        RuleFor(x => x.Department)
            .MaximumLength(50).WithMessage("Department must be 50 characters or less")
            .When(x => !string.IsNullOrEmpty(x.Department));

        RuleFor(x => x.Activity)
            .MaximumLength(50).WithMessage("Activity must be 50 characters or less")
            .When(x => !string.IsNullOrEmpty(x.Activity));

        RuleFor(x => x.CountryCode)
            .MaximumLength(5).WithMessage("Country code must be 5 characters or less")
            .When(x => !string.IsNullOrEmpty(x.CountryCode));

        RuleFor(x => x.DCN)
            .MaximumLength(50).WithMessage("DCN must be 50 characters or less")
            .When(x => !string.IsNullOrEmpty(x.DCN));

        RuleFor(x => x.Extra)
            .MaximumLength(500).WithMessage("Extra information must be 500 characters or less")
            .When(x => !string.IsNullOrEmpty(x.Extra));

        RuleFor(x => x.WarrantyDate)
            .GreaterThan(DateTime.Now.AddYears(-50)).WithMessage("Warranty date cannot be more than 50 years in the past")
            .LessThan(DateTime.Now.AddYears(20)).WithMessage("Warranty date cannot be more than 20 years in the future")
            .When(x => x.WarrantyDate.HasValue);
    }
}
