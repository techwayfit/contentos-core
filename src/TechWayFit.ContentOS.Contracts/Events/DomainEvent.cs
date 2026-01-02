namespace TechWayFit.ContentOS.Contracts.Events;

/// <summary>
/// Base event for all domain events in the system
/// </summary>
public abstract record DomainEvent
{
    public string EventId { get; init; } = Guid.NewGuid().ToString();
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;
    public string TenantId { get; init; } = string.Empty;
}
