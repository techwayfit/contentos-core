using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;
using TechWayFit.ContentOS.Tenancy.Domain.Identity;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Identity;

public class GroupRepository : EfCoreRepository<Group, GroupRow, Guid>, IGroupRepository
{
    public GroupRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override Group MapToDomain(GroupRow row)
    {
        return new Group
        {
            Id = row.Id,
            TenantId = row.TenantId,
            Name = row.Name,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override GroupRow MapToRow(Group entity)
    {
        var row = new GroupRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            Name = entity.Name
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<GroupRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<Group?> GetByNameAsync(Guid tenantId, string name, CancellationToken cancellationToken = default)
    {
        var row = await Context.Set<GroupRow>()
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.Name == name, cancellationToken);
        return row != null ? MapToDomain(row) : null;
    }

    public async Task<IReadOnlyList<Group>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var rows = await Context.Set<GroupRow>()
            .Where(r => r.TenantId == tenantId)
            .ToListAsync(cancellationToken);
        return rows.Select(MapToDomain).ToList();
    }

    public Task<bool> NameExistsAsync(Guid tenantId, string name, CancellationToken cancellationToken = default)
    {
        return Context.Set<GroupRow>()
            .AnyAsync(r => r.TenantId == tenantId && r.Name == name, cancellationToken);
    }
 
}
