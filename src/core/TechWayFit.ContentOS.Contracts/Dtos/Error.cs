namespace TechWayFit.ContentOS.Contracts.Dtos;

/// <summary>
/// Common error response
/// </summary>
public record Error
{
    public required string Code { get; init; }
    public required string Message { get; init; }
    public Dictionary<string, string[]>? ValidationErrors { get; init; }

    public static Error Validation(string message, Dictionary<string, string[]>? errors = null) =>
        new() { Code = "VALIDATION_ERROR", Message = message, ValidationErrors = errors };

    public static Error NotFound(string message) =>
        new() { Code = "NOT_FOUND", Message = message };

    public static Error Conflict(string message) =>
        new() { Code = "CONFLICT", Message = message };

    public static Error Unauthorized(string message) =>
        new() { Code = "UNAUTHORIZED", Message = message };

    public static Error Forbidden(string message) =>
        new() { Code = "FORBIDDEN", Message = message };

    public static Error Internal(string message) =>
        new() { Code = "INTERNAL_ERROR", Message = message };
}
