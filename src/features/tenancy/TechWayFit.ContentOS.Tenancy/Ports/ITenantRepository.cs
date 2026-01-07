using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Tenancy.Domain;

namespace TechWayFit.ContentOS.Tenancy.Ports;

/// <summary>
/// Repository interface for Tenant entity persistence
/// </summary>
public interface ITenantRepository : IRepository<Tenant, Guid>
{
    /// <summary>
    /// Get tenant by unique key (slug)
    /// </summary>
    Task<Tenant?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a tenant key already exists
    /// </summary>
    Task<bool> KeyExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// List tenants by status
    /// </summary>
    /// <param name="status">Tenant status enum value</param>
    Task<IReadOnlyList<Tenant>> ListByStatusAsync(TenantStatus status, CancellationToken cancellationToken = default);
}
