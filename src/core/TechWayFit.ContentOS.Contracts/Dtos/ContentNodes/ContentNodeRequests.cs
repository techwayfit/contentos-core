namespace TechWayFit.ContentOS.Contracts.Dtos.ContentNodes;

/// <summary>
/// Request to create a content node
/// </summary>
public record CreateContentNodeRequest
{
    /// <summary>
    /// Site ID this node belongs to
    /// </summary>
    public required Guid SiteId { get; init; }

    /// <summary>
    /// Parent node ID (null for root nodes)
    /// </summary>
    public Guid? ParentId { get; init; }

    /// <summary>
    /// Content item ID this node represents (optional)
    /// </summary>
 public Guid? ContentItemId { get; init; }

    /// <summary>
    /// URL slug for this node
    /// </summary>
    public required string Slug { get; init; }

    /// <summary>
    /// Sort order among siblings (optional, auto-assigned if not provided)
    /// </summary>
    public int? SortOrder { get; init; }
}

/// <summary>
/// Request to update a content node
/// </summary>
public record UpdateContentNodeRequest
{
    /// <summary>
    /// URL slug for this node
  /// </summary>
    public required string Slug { get; init; }

    /// <summary>
    /// Content item ID this node represents (optional)
    /// </summary>
    public Guid? ContentItemId { get; init; }
}

/// <summary>
/// Response containing content node details
/// </summary>
public record ContentNodeResponse
{
    public Guid Id { get; init; }
    public Guid SiteId { get; init; }
    public Guid? ParentId { get; init; }
    public Guid? ContentItemId { get; init; }
    public string Slug { get; init; } = string.Empty;
    public int SortOrder { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}
