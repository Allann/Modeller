using ValidationResult = FluentValidation.Results.ValidationResult;

namespace JJs.UnitsManagement.Sdk.Common;

/// <summary>
/// Represents the result of an API operation with success/failure states
/// </summary>
/// <typeparam name="T">The type of data returned on success</typeparam>
public record ApiResult<T>
{
    /// <summary>
    /// Indicates whether the operation was successful
    /// </summary>
    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; init; }

    /// <summary>
    /// The data returned on successful operations
    /// </summary>
    [JsonPropertyName("data")]
    public T? Data { get; init; }

    /// <summary>
    /// Error message for failed operations
    /// </summary>
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Validation errors if the failure was due to validation
    /// </summary>
    [JsonPropertyName("validationErrors")]
    public IReadOnlyList<string>? ValidationErrors { get; init; }

    /// <summary>
    /// Creates a successful result with data
    /// </summary>
    /// <param name="data">The successful operation data</param>
    /// <returns>A successful ApiResult</returns>
    public static ApiResult<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };

    /// <summary>
    /// Creates a failed result with an error message
    /// </summary>
    /// <param name="errorMessage">The error message</param>
    /// <returns>A failed ApiResult</returns>
    public static ApiResult<T> Failure(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };

    /// <summary>
    /// Creates a failed result with validation errors
    /// </summary>
    /// <param name="validationResult">The validation result containing errors</param>
    /// <returns>A failed ApiResult with validation errors</returns>
    public static ApiResult<T> ValidationFailure(ValidationResult validationResult) => new()
    {
        IsSuccess = false,
        ErrorMessage = "Validation failed",
        ValidationErrors = validationResult.Errors.Select(e => e.ErrorMessage).ToArray()
    };
}
