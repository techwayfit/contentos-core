namespace TechWayFit.ContentOS.Contracts.Dtos.Routes;

/// <summary>
/// Request to create a route
/// </summary>
public record CreateRouteRequest
{
    /// <summary>
    /// Site ID
    /// </summary>
    public required Guid SiteId { get; init; }

    /// <summary>
    /// Content node ID
    /// </summary>
    public required Guid NodeId { get; init; }

    /// <summary>
    /// Route path (e.g., "/blog/my-post")
    /// </summary>
    public required string RoutePath { get; init; }

    /// <summary>
    /// Whether this is the primary route for the node
  /// </summary>
    public bool IsPrimary { get; init; }
}

/// <summary>
/// Response containing route details
/// </summary>
public record RouteResponse
{
    public Guid Id { get; init; }
    public Guid SiteId { get; init; }
    public Guid NodeId { get; init; }
public string RoutePath { get; init; } = string.Empty;
    public bool IsPrimary { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
