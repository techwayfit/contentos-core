namespace TechWayFit.ContentOS.Contracts.Dtos;

/// <summary>
/// Request to create new content
/// </summary>
public record CreateContentRequest
{
    public required string ContentType { get; init; }
    public required string LanguageCode { get; init; }
    public required string Title { get; init; }
    public required string Slug { get; init; }
    public Dictionary<string, object> Fields { get; init; } = new();
}
