namespace TechWayFit.ContentOS.Contracts.Events;

/// <summary>
/// Event raised when content is created
/// </summary>
public record ContentCreatedEvent : DomainEvent
{
    public required Guid ContentId { get; init; }
    public required string ContentType { get; init; }
    public required string LanguageCode { get; init; }
    public required string Title { get; init; }
}
