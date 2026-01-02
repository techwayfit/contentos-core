namespace TechWayFit.ContentOS.Abstractions;

/// <summary>
/// Represents the result of an operation that can succeed with a value or fail with an error
/// </summary>
public abstract record Result<TValue, TError>
{
    private Result() { } // Prevent external inheritance

    /// <summary>
    /// Represents a successful result containing a value
    /// </summary>
    public sealed record Success(TValue Value) : Result<TValue, TError>;

    /// <summary>
    /// Represents a failed result containing an error
    /// </summary>
    public sealed record Failure(TError Error) : Result<TValue, TError>;

    /// <summary>
    /// True if the result is successful
    /// </summary>
    public bool IsSuccess => this is Success;

    /// <summary>
    /// True if the result is a failure
    /// </summary>
    public bool IsFailure => this is Failure;

    /// <summary>
    /// Pattern matching helper for result handling
    /// </summary>
    public TResult Match<TResult>(
        Func<TValue, TResult> onSuccess,
        Func<TError, TResult> onFailure) =>
        this switch
        {
            Success success => onSuccess(success.Value),
            Failure failure => onFailure(failure.Error),
            _ => throw new InvalidOperationException("Unknown result type")
        };
}

/// <summary>
/// Factory methods for creating Result instances
/// </summary>
public static class Result
{
    /// <summary>
    /// Creates a successful result
    /// </summary>
    public static Result<TValue, TError> Ok<TValue, TError>(TValue value) =>
        new Result<TValue, TError>.Success(value);

    /// <summary>
    /// Creates a failed result
    /// </summary>
    public static Result<TValue, TError> Fail<TValue, TError>(TError error) =>
        new Result<TValue, TError>.Failure(error);
}
