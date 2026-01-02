namespace TechWayFit.ContentOS.Contracts.Events;

/// <summary>
/// Integration event V1 - raised when content is published and ready for public consumption
/// This is a versioned external message contract, NOT a domain event.
/// </summary>
public record ContentPublishedEventV1 : IntegrationEvent
{
    public required Guid ContentId { get; init; }
    public required string ContentType { get; init; }
    public required string LanguageCode { get; init; }
    public required string Slug { get; init; }
    public DateTimeOffset PublishedAt { get; init; } = DateTimeOffset.UtcNow;
}
