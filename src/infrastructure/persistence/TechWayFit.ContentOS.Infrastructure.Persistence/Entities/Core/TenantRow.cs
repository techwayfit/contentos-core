namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;

/// <summary>
/// Database row entity for Tenant
/// Provider-agnostic - can be used with any EF Core provider
/// Note: Tenants are NOT tenant-scoped (they ARE the tenants)
/// </summary>
public sealed class TenantRow
{
    public Guid Id { get; set; }

    /// <summary>
    /// Unique tenant key (slug) - used in URLs and API routing
    /// Example: "techwayfit", "acme-corp"
    /// </summary>
    public string Key { get; set; } = default!;

    /// <summary>
    /// Tenant display name
    /// Example: "TechWayFit", "ACME Corporation"
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Tenant status (0=Active, 1=Suspended, 2=Inactive)
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// When the tenant was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the tenant was last updated
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}
