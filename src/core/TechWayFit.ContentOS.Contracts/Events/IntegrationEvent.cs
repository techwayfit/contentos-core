namespace TechWayFit.ContentOS.Contracts.Events;

/// <summary>
/// Base class for all integration/public events (external message contract)
/// These are NOT domain events - they are versioned integration contracts for cross-boundary communication.
/// Includes tenant context and user context for full auditability
/// </summary>
public abstract record IntegrationEvent
{
    /// <summary>
    /// Unique identifier for this event instance
    /// </summary>
    public Guid EventId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Timestamp when the event occurred
    /// </summary>
    public DateTimeOffset OccurredAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Tenant identifier for multi-tenant isolation
    /// </summary>
    public required Guid TenantId { get; init; }

    /// <summary>
    /// Site identifier within the tenant
    /// </summary>
    public required Guid SiteId { get; init; }

    /// <summary>
    /// Environment where the event occurred
    /// </summary>
    public required string Environment { get; init; }

    /// <summary>
    /// User who triggered the event (if authenticated)
    /// </summary>
    public Guid? UserId { get; init; }
}

