using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Tenancy.Domain.Identity;

namespace TechWayFit.ContentOS.Tenancy.Ports.Identity;

/// <summary>
/// Repository interface for Role entity persistence
/// </summary>
public interface IRoleRepository : IRepository<Role, Guid>
{
    Task<Role?> GetByNameAsync(Guid tenantId, string name, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(Guid tenantId, string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
