namespace TechWayFit.ContentOS.Contracts.Events;

/// <summary>
/// Integration event V1 - raised when content workflow state transitions
/// This is a versioned external message contract, NOT a domain event.
/// </summary>
public record WorkflowTransitionedEventV1 : IntegrationEvent
{
    public required Guid ContentId { get; init; }
    public required string FromState { get; init; }
    public required string ToState { get; init; }
    public string? Comment { get; init; }
    public DateTimeOffset TransitionedAt { get; init; } = DateTimeOffset.UtcNow;
}
