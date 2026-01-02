namespace TechWayFit.ContentOS.Abstractions;

/// <summary>
/// Provides tenant isolation context for multi-tenant operations
/// </summary>
public interface ITenantContext
{
    string TenantId { get; }
    string SiteId { get; }
    string Environment { get; }
}
