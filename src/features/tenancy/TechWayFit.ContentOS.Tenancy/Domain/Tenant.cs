namespace TechWayFit.ContentOS.Tenancy.Domain;

/// <summary>
/// Tenant domain entity - Pure POCO
/// Represents the top-level isolation boundary in multi-tenant architecture
/// Validation is handled at the use-case/API layer
/// </summary>
public sealed class Tenant
{
    public Guid Id { get; set; }

    /// <summary>
    /// Unique tenant key (slug) - used in URLs, API calls, etc.
    /// Example: "techwayfit", "acme-corp"
    /// </summary>
    public string Key { get; set; } = default!;

    /// <summary>
    /// Human-readable tenant name
    /// Example: "TechWayFit", "ACME Corporation"
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Tenant status (Active/Suspended/Inactive)
    /// </summary>
    public TenantStatus Status { get; set; }

    /// <summary>
    /// When the tenant was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the tenant was last updated
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}
