namespace TechWayFit.ContentOS.Contracts.Events;

/// <summary>
/// Event raised when content workflow state transitions
/// </summary>
public record WorkflowTransitionedEvent : DomainEvent
{
    public required Guid ContentId { get; init; }
    public required string FromState { get; init; }
    public required string ToState { get; init; }
    public string? Comment { get; init; }
    public DateTimeOffset TransitionedAt { get; init; } = DateTimeOffset.UtcNow;
}
