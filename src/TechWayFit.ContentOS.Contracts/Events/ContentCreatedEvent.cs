namespace TechWayFit.ContentOS.Contracts.Events;

/// <summary>
/// Event raised when content is created
/// </summary>
public record ContentCreatedEvent : DomainEvent
{
    public string ContentId { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public string CreatedBy { get; init; } = string.Empty;
}
