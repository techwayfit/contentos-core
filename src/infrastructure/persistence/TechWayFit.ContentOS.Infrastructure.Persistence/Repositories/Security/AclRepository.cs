using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Security;
using TechWayFit.ContentOS.Kernel.Domain.Security;
using TechWayFit.ContentOS.Kernel.Ports.Security;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Security;

public class AclRepository : EfCoreRepository<AclEntry, AclEntryRow, Guid>, IAclRepository
{
    public AclRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override AclEntry MapToDomain(AclEntryRow row)
    {
        return new AclEntry
        {
            Id = row.Id,
            TenantId = row.TenantId,
            ScopeType = row.ScopeType,
            ScopeId = row.ScopeId,
            PrincipalType = row.PrincipalType,
            PrincipalId = row.PrincipalId,
            Effect = row.Effect,
            ActionsCsv = row.ActionsCsv,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override AclEntryRow MapToRow(AclEntry entity)
    {
        var row = new AclEntryRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            ScopeType = entity.ScopeType,
            ScopeId = entity.ScopeId,
            PrincipalType = entity.PrincipalType,
            PrincipalId = entity.PrincipalId,
            Effect = entity.Effect,
            ActionsCsv = entity.ActionsCsv
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<AclEntryRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<IEnumerable<AclEntry>> GetByScopeAsync(Guid tenantId, string scopeType, Guid scopeId)
    {
        var rows = await DbSet
            .Where(r => r.TenantId == tenantId && r.ScopeType == scopeType && r.ScopeId == scopeId)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task<IEnumerable<AclEntry>> GetByPrincipalAsync(Guid tenantId, string principalType, Guid principalId)
    {
        var rows = await DbSet
            .Where(r => r.TenantId == tenantId && r.PrincipalType == principalType && r.PrincipalId == principalId)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task<bool> CheckPermissionAsync(Guid tenantId, string scopeType, Guid scopeId, string principalType, Guid principalId, string action)
    {
        var hasPermission = await DbSet
            .AnyAsync(r => r.TenantId == tenantId 
                && r.ScopeType == scopeType 
                && r.ScopeId == scopeId
                && r.PrincipalType == principalType 
                && r.PrincipalId == principalId
                && r.Effect == "Allow"
                && r.ActionsCsv.Contains(action));
        
        return hasPermission;
    }
}
