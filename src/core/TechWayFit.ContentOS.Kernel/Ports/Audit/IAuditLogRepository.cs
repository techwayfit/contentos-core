using TechWayFit.ContentOS.Abstractions.Repositories;

namespace TechWayFit.ContentOS.Kernel.Ports.Audit;

public interface IAuditLogRepository : IRepository<Domain.Audit.AuditLog, Guid>
{
    Task<IEnumerable<Domain.Audit.AuditLog>> GetByEntityAsync(Guid tenantId, string entityType, Guid entityId);
    Task<IEnumerable<Domain.Audit.AuditLog>> GetByActorAsync(Guid tenantId, Guid actorUserId);
    Task<IEnumerable<Domain.Audit.AuditLog>> GetByDateRangeAsync(Guid tenantId, DateTime startDate, DateTime endDate);
}
