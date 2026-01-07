using TechWayFit.ContentOS.Abstractions.Repositories;

namespace TechWayFit.ContentOS.Kernel.Ports.Security;

public interface IAclRepository : IRepository<Domain.Security.AclEntry, Guid>
{
    Task<IEnumerable<Domain.Security.AclEntry>> GetByScopeAsync(Guid tenantId, string scopeType, Guid scopeId);
    Task<IEnumerable<Domain.Security.AclEntry>> GetByPrincipalAsync(Guid tenantId, string principalType, Guid principalId);
    Task<bool> CheckPermissionAsync(Guid tenantId, string scopeType, Guid scopeId, string principalType, Guid principalId, string action);
}
