using TechWayFit.ContentOS.Tenancy.Domain;

namespace TechWayFit.ContentOS.Tenancy.Ports;

/// <summary>
/// Repository contract for Tenant persistence
/// </summary>
public interface ITenantRepository
{
    /// <summary>
    /// Add a new tenant
    /// </summary>
    Task<Guid> AddAsync(Tenant tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get tenant by ID
    /// </summary>
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get tenant by unique key
    /// </summary>
    Task<Tenant?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update existing tenant
    /// </summary>
    Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default);

    /// <summary>
    /// List all tenants with optional filtering
    /// </summary>
    Task<IReadOnlyList<Tenant>> ListAsync(TenantStatus? status = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a tenant key already exists
    /// </summary>
    Task<bool> KeyExistsAsync(string key, CancellationToken cancellationToken = default);
}
