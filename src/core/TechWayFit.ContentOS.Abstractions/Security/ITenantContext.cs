namespace TechWayFit.ContentOS.Abstractions.Security;

/// <summary>
/// Provides access to the current tenant context
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// Current tenant ID (resolved from request)
    /// </summary>
Guid CurrentTenantId { get; }

    /// <summary>
    /// Current tenant key (optional, for display/routing)
    /// </summary>
    string? CurrentTenantKey { get; }
 
    /// <summary>
    /// Whether tenant context is resolved
    /// </summary>
    bool IsResolved { get; }
}
