using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Tenancy.Domain.Core;

namespace TechWayFit.ContentOS.Tenancy.Ports.Core;

/// <summary>
/// Repository interface for Site entity persistence
/// </summary>
public interface ISiteRepository : IRepository<Site, Guid>
{
    Task<Site?> GetByHostNameAsync(Guid tenantId, string hostName, CancellationToken cancellationToken = default);
    Task<bool> HostNameExistsAsync(Guid tenantId, string hostName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Site>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
