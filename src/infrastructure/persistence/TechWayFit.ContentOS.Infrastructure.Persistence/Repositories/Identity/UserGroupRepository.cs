using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;
using TechWayFit.ContentOS.Tenancy.Domain.Identity;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Identity;

public class UserGroupRepository : EfCoreRepository<UserGroup, UserGroupRow, Guid>, IUserGroupRepository
{
    public UserGroupRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override UserGroup MapToDomain(UserGroupRow row)
    {
        return new UserGroup
        {
            Id = row.Id,
            TenantId = row.TenantId,
            UserId = row.UserId,
            GroupId = row.GroupId,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override UserGroupRow MapToRow(UserGroup entity)
    {
        var row = new UserGroupRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            UserId = entity.UserId,
            GroupId = entity.GroupId
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<UserGroupRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<IEnumerable<UserGroup>> GetByUserAsync(Guid tenantId, Guid userId)
    {
        var rows = await Context.Set<UserGroupRow>()
            .Where(r => r.TenantId == tenantId && r.UserId == userId)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task<IEnumerable<UserGroup>> GetByGroupAsync(Guid tenantId, Guid groupId)
    {
        var rows = await Context.Set<UserGroupRow>()
            .Where(r => r.TenantId == tenantId && r.GroupId == groupId)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task AssignGroupAsync(Guid tenantId, Guid userId, Guid groupId, Guid? createdBy = null)
    {
        var userGroup = new UserGroup
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            GroupId = groupId,
            
        };
        if(null!=createdBy)
        {
            userGroup.Audit.CreatedBy = createdBy.Value;
            userGroup.Audit.CreatedOn = DateTime.UtcNow;
        }
        await AddAsync(userGroup);
    }

    public async Task RemoveGroupAsync(Guid tenantId, Guid userId, Guid groupId)
    {
        var row = await Context.Set<UserGroupRow>()
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.UserId == userId && r.GroupId == groupId);
        
        if (row != null)
        {
            Context.Set<UserGroupRow>().Remove(row);
        }
    }
}
