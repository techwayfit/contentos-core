using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Audit;
using TechWayFit.ContentOS.Kernel.Domain.Audit;
using TechWayFit.ContentOS.Kernel.Ports.Audit;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Audit;

public class AuditLogRepository : EfCoreRepository<AuditLog, AuditLogRow, Guid>, IAuditLogRepository
{
    public AuditLogRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override AuditLog MapToDomain(AuditLogRow row)
    {
        return new AuditLog
        {
            Id = row.Id,
            TenantId = row.TenantId,
            ActorUserId = row.ActorUserId,
            ActionKey = row.ActionKey,
            EntityType = row.EntityType,
            EntityId = row.EntityId,
            DetailsJson = row.DetailsJson,
            CreatedOn = row.CreatedOn
        };
    }

    protected override AuditLogRow MapToRow(AuditLog entity)
    {
        return new AuditLogRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            ActorUserId = entity.ActorUserId,
            ActionKey = entity.ActionKey,
            EntityType = entity.EntityType,
            EntityId = entity.EntityId,
            DetailsJson = entity.DetailsJson,
            CreatedOn = entity.CreatedOn
        };
    }

    protected override Expression<Func<AuditLogRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<IEnumerable<AuditLog>> GetByEntityAsync(Guid tenantId, string entityType, Guid entityId)
    {
        var rows = await Context.Set<AuditLogRow>()
            .Where(r => r.TenantId == tenantId && r.EntityType == entityType && r.EntityId == entityId)
            .OrderByDescending(r => r.CreatedOn)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task<IEnumerable<AuditLog>> GetByActorAsync(Guid tenantId, Guid actorUserId)
    {
        var rows = await Context.Set<AuditLogRow>()
            .Where(r => r.TenantId == tenantId && r.ActorUserId == actorUserId)
            .OrderByDescending(r => r.CreatedOn)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(Guid tenantId, DateTime startDate, DateTime endDate)
    {
        var rows = await Context.Set<AuditLogRow>()
            .Where(r => r.TenantId == tenantId && r.CreatedOn >= startDate && r.CreatedOn <= endDate)
            .OrderByDescending(r => r.CreatedOn)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }
}
