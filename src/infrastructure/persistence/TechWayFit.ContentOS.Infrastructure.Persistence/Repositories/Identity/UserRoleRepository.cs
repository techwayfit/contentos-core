using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;
using TechWayFit.ContentOS.Tenancy.Domain.Identity;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Identity;

public class UserRoleRepository : EfCoreRepository<UserRole, UserRoleRow, Guid>, IUserRoleRepository
{
    public UserRoleRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override UserRole MapToDomain(UserRoleRow row)
    {
        return new UserRole
        {
            Id = row.Id,
            TenantId = row.TenantId,
            UserId = row.UserId,
            RoleId = row.RoleId,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override UserRoleRow MapToRow(UserRole entity)
    {
        var row = new UserRoleRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            UserId = entity.UserId,
            RoleId = entity.RoleId
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<UserRoleRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<IEnumerable<UserRole>> GetByUserAsync(Guid tenantId, Guid userId)
    {
        var rows = await Context.Set<UserRoleRow>()
            .Where(r => r.TenantId == tenantId && r.UserId == userId)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task<IEnumerable<UserRole>> GetByRoleAsync(Guid tenantId, Guid roleId)
    {
        var rows = await Context.Set<UserRoleRow>()
            .Where(r => r.TenantId == tenantId && r.RoleId == roleId)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task AssignRoleAsync(Guid tenantId, Guid userId, Guid roleId, Guid? createdBy = null)
    {
        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            RoleId = roleId
        };
        if(null!=createdBy)
        {
            userRole.Audit.CreatedBy = createdBy.Value;
            userRole.Audit.CreatedOn = DateTime.UtcNow;
        }
        await AddAsync(userRole);
    }

    public async Task RemoveRoleAsync(Guid tenantId, Guid userId, Guid roleId)
    {
        var row = await Context.Set<UserRoleRow>()
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.UserId == userId && r.RoleId == roleId);
        
        if (row != null)
        {
            Context.Set<UserRoleRow>().Remove(row);
        }
    }
}
