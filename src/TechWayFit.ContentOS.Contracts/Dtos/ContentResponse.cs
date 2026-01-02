namespace TechWayFit.ContentOS.Contracts.Dtos;

/// <summary>
/// Response containing content item details
/// </summary>
public record ContentResponse
{
    public required Guid Id { get; init; }
    public required string ContentType { get; init; }
    public required string LanguageCode { get; init; }
    public required string DefaultLanguage { get; init; }
    public required string Title { get; init; }
    public required string Slug { get; init; }
    public required string WorkflowStatus { get; init; }
    public Dictionary<string, object> Fields { get; init; } = new();
    public List<string> AvailableLanguages { get; init; } = new();
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
    public Guid? CreatedBy { get; init; }
}
