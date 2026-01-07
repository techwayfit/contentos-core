using TechWayFit.ContentOS.Abstractions.Repositories;

namespace TechWayFit.ContentOS.Tenancy.Ports.Identity;

public interface IUserGroupRepository : IRepository<Domain.Identity.UserGroup, Guid>
{
    Task<IEnumerable<Domain.Identity.UserGroup>> GetByUserAsync(Guid tenantId, Guid userId);
    Task<IEnumerable<Domain.Identity.UserGroup>> GetByGroupAsync(Guid tenantId, Guid groupId);
    Task AssignGroupAsync(Guid tenantId, Guid userId, Guid groupId, Guid? createdBy = null);
    Task RemoveGroupAsync(Guid tenantId, Guid userId, Guid groupId);
}
