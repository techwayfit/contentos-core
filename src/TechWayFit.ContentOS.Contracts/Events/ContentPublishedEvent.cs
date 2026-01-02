namespace TechWayFit.ContentOS.Contracts.Events;

/// <summary>
/// Event raised when content is published and ready for public consumption
/// </summary>
public record ContentPublishedEvent : DomainEvent
{
    public required Guid ContentId { get; init; }
    public required string ContentType { get; init; }
    public required string LanguageCode { get; init; }
    public required string Slug { get; init; }
    public DateTimeOffset PublishedAt { get; init; } = DateTimeOffset.UtcNow;
}
