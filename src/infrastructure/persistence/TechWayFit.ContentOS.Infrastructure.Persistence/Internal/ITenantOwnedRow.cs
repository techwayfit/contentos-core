namespace TechWayFit.ContentOS.Infrastructure.Persistence.Internal;

/// <summary>
/// Marker interface for database row entities that belong to a tenant
/// Ensures tenant_id is present on all multi-tenant tables
/// </summary>
public interface ITenantOwnedRow
{
    /// <summary>
    /// The tenant that owns this row
    /// </summary>
    Guid TenantId { get; set; }

    /// <summary>
    /// The site within the tenant (optional multi-level hierarchy)
    /// </summary>
    Guid SiteId { get; set; }

    /// <summary>
    /// The environment (e.g., "production", "staging", "development")
    /// </summary>
    string Environment { get; set; }
}
