using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;
using TechWayFit.ContentOS.Tenancy.Domain.Identity;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Identity;

public class RoleRepository : EfCoreRepository<Role, RoleRow, Guid>, IRoleRepository
{
    public RoleRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override Role MapToDomain(RoleRow row)
    {
        return new Role
        {
            Id = row.Id,
            TenantId = row.TenantId,
            Name = row.Name,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override RoleRow MapToRow(Role entity)
    {
        var row = new RoleRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            Name = entity.Name
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<RoleRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<Role?> GetByNameAsync(Guid tenantId, string name, CancellationToken cancellationToken = default)
    {
        var row = await Context.Set<RoleRow>()
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.Name == name, cancellationToken);
        return row != null ? MapToDomain(row) : null;
    }

    public async Task<IReadOnlyList<Role>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var rows = await Context.Set<RoleRow>()
            .Where(r => r.TenantId == tenantId)
            .ToListAsync(cancellationToken);
        return rows.Select(MapToDomain).ToList();
    }

    public Task<bool> NameExistsAsync(Guid tenantId, string name, CancellationToken cancellationToken = default)
    {
        return Context.Set<RoleRow>()   
            .AnyAsync(r => r.TenantId == tenantId && r.Name == name, cancellationToken);        
    }
}
