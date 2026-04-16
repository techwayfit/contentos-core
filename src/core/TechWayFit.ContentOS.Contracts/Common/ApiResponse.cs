namespace TechWayFit.ContentOS.Contracts.Common;

/// <summary>
/// Standard API response wrapper
/// </summary>
public record ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResponse(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message
    };

    public static ApiResponse<T> FailureResponse(string error) => new()
    {
        Success = false,
        Errors = new[] { error }
    };

    public static ApiResponse<T> FailureResponse(IReadOnlyList<string> errors) => new()
    {
        Success = false,
      Errors = errors
    };
}

/// <summary>
/// Paginated response wrapper
/// </summary>
public record PagedResponse<T>
{
 public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
