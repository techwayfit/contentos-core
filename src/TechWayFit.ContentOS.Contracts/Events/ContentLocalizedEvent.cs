namespace TechWayFit.ContentOS.Contracts.Events;

/// <summary>
/// Event raised when a localization is added to content
/// </summary>
public record ContentLocalizedEvent : DomainEvent
{
    public required Guid ContentId { get; init; }
    public required string LanguageCode { get; init; }
    public required string Title { get; init; }
}
