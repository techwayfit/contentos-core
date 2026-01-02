namespace TechWayFit.ContentOS.Contracts.Events;

/// <summary>
/// Integration event V1 - raised when content is created
/// This is a versioned external message contract, NOT a domain event.
/// </summary>
public record ContentCreatedEventV1 : IntegrationEvent
{
    public required Guid ContentId { get; init; }
    public required string ContentType { get; init; }
    public required string LanguageCode { get; init; }
    public required string Title { get; init; }
}
