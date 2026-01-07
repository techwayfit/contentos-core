using TechWayFit.ContentOS.Abstractions.Repositories;

namespace TechWayFit.ContentOS.Tenancy.Ports.Identity;

public interface IUserRoleRepository : IRepository<Domain.Identity.UserRole, Guid>
{
    Task<IEnumerable<Domain.Identity.UserRole>> GetByUserAsync(Guid tenantId, Guid userId);
    Task<IEnumerable<Domain.Identity.UserRole>> GetByRoleAsync(Guid tenantId, Guid roleId);
    Task AssignRoleAsync(Guid tenantId, Guid userId, Guid roleId, Guid? createdBy = null);
    Task RemoveRoleAsync(Guid tenantId, Guid userId, Guid roleId);
}
