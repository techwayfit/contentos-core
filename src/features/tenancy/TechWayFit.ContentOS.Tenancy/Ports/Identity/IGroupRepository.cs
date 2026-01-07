using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Tenancy.Domain.Identity;

namespace TechWayFit.ContentOS.Tenancy.Ports.Identity;

/// <summary>
/// Repository interface for Group entity persistence
/// </summary>
public interface IGroupRepository : IRepository<Group, Guid>
{
    Task<Group?> GetByNameAsync(Guid tenantId, string name, CancellationToken cancellationToken = default);
    Task<bool> NameExistsAsync(Guid tenantId, string name, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Group>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
