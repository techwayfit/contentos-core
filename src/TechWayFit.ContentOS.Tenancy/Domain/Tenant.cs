namespace TechWayFit.ContentOS.Tenancy.Domain;

/// <summary>
/// Tenant domain entity
/// Represents the top-level isolation boundary in multi-tenant architecture
/// </summary>
public sealed class Tenant
{
    public Guid Id { get; private set; }

    /// <summary>
    /// Unique tenant key (slug) - used in URLs, API calls, etc.
    /// Example: "techwayfit", "acme-corp"
    /// </summary>
    public string Key { get; private set; } = default!;

    /// <summary>
    /// Human-readable tenant name
    /// Example: "TechWayFit", "ACME Corporation"
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// Tenant status (Active/Disabled)
    /// </summary>
    public TenantStatus Status { get; private set; }

    /// <summary>
    /// When the tenant was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// When the tenant was last updated
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; private set; }

    // Private constructor for EF Core
    private Tenant() { }

    /// <summary>
    /// Create a new tenant
    /// </summary>
    public static Tenant Create(string key, string name)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Tenant key cannot be empty", nameof(key));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tenant name cannot be empty", nameof(name));

        // Validate key format: lowercase alphanumeric with hyphens
        if (!System.Text.RegularExpressions.Regex.IsMatch(key, "^[a-z0-9-]+$"))
            throw new ArgumentException("Tenant key must be lowercase alphanumeric with hyphens only", nameof(key));

        return new Tenant
        {
            Id = Guid.NewGuid(),
            Key = key,
            Name = name,
            Status = TenantStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Update tenant details
    /// </summary>
    public void Update(string name, TenantStatus status)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tenant name cannot be empty", nameof(name));

        Name = name;
        Status = status;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Disable the tenant
    /// </summary>
    public void Disable()
    {
        Status = TenantStatus.Disabled;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Activate the tenant
    /// </summary>
    public void Activate()
    {
        Status = TenantStatus.Active;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
