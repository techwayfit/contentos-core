namespace TechWayFit.ContentOS.Contracts.Dtos.ContentVersions;

/// <summary>
/// Request to create a new version
/// </summary>
public record CreateVersionRequest
{
  /// <summary>
    /// Whether to copy field values from latest version
    /// </summary>
    public bool CopyFromLatest { get; init; } = true;
}

/// <summary>
/// Response containing content version details
/// </summary>
public record ContentVersionResponse
{
    public Guid Id { get; init; }
    public Guid ContentItemId { get; init; }
    public int VersionNumber { get; init; }
    public string Lifecycle { get; init; } = string.Empty;
    public Guid? WorkflowStateId { get; init; }
    public DateTime? PublishedAt { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}
