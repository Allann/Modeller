namespace JJs.UnitsManagement.Sdk.Sync;

/// <summary>
/// Validator for StartSyncJobRequest
/// </summary>
public class StartSyncJobRequestValidator : AbstractValidator<StartSyncJobRequest>
{
    /// <summary>
    /// Initializes a new instance of the StartSyncJobRequestValidator
    /// </summary>
    public StartSyncJobRequestValidator()
    {
        RuleFor(x => x.JobName)
            .MaximumLength(100)
            .WithMessage("Job name cannot exceed 100 characters");

        RuleFor(x => x)
            .Must(HaveAtLeastOneOperationTypeSelected)
            .WithMessage("At least one operation type (CreateNew or UpdateExisting) must be selected")
            .WithName("OperationSelection");
    }

    /// <summary>
    /// Validates that at least one operation type is selected
    /// </summary>
    /// <param name="request">The sync request</param>
    /// <returns>True if at least one operation type is selected</returns>
    private static bool HaveAtLeastOneOperationTypeSelected(StartSyncJobRequest request)
    {
        return request.CreateNew || request.UpdateExisting;
    }
}
