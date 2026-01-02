namespace TechWayFit.ContentOS.Contracts.Dtos;

/// <summary>
/// Content item data transfer object
/// </summary>
public record ContentDto
{
    public string Id { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public Dictionary<string, object> Fields { get; init; } = new();
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}
