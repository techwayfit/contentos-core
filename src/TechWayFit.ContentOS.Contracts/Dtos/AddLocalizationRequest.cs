namespace TechWayFit.ContentOS.Contracts.Dtos;

/// <summary>
/// Request to add localization to existing content
/// </summary>
public record AddLocalizationRequest
{
    public required string LanguageCode { get; init; }
    public required string Title { get; init; }
    public required string Slug { get; init; }
    public Dictionary<string, object> Fields { get; init; } = new();
}
