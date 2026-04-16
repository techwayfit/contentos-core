namespace TechWayFit.ContentOS.Contracts.Dtos.ContentTypes;

/// <summary>
/// Request to create a new content type
/// </summary>
public record CreateContentTypeRequest
{
    /// <summary>
    /// Unique identifier key for the content type (e.g., "blog-post", "product")
    /// </summary>
    public required string TypeKey { get; init; }

  /// <summary>
    /// Human-readable display name
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Optional description of the content type
    /// </summary>
    public string? Description { get; init; }
}

/// <summary>
/// Request to update an existing content type
/// </summary>
public record UpdateContentTypeRequest
{
    /// <summary>
    /// Human-readable display name
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Optional description of the content type
    /// </summary>
    public string? Description { get; init; }
}

/// <summary>
/// Response containing content type details
/// </summary>
public record ContentTypeResponse
{
    public Guid Id { get; init; }
    public string TypeKey { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public int SchemaVersion { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}
