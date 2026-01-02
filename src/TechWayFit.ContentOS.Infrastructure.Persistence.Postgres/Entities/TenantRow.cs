namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;

/// <summary>
/// Database row for Tenant entity
/// Note: Tenants are NOT tenant-scoped (they ARE the tenants)
/// </summary>
public sealed class TenantRow
{
    public Guid Id { get; set; }

    /// <summary>
    /// Unique tenant key (slug)
    /// </summary>
    public string Key { get; set; } = default!;

    /// <summary>
    /// Tenant display name
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Tenant status (0=Active, 1=Disabled)
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
