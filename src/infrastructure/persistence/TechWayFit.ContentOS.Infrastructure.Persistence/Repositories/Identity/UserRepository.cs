using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;
using TechWayFit.ContentOS.Tenancy.Domain.Identity;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Identity;

public class UserRepository : EfCoreRepository<User, UserRow, Guid>, IUserRepository
{
    public UserRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override User MapToDomain(UserRow row)
    {
        return new User
        {
            Id = row.Id,
            TenantId = row.TenantId,
            Email = row.Email,
            DisplayName = row.DisplayName,
            Status = row.Status,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override UserRow MapToRow(User entity)
    {
        var row = new UserRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            Email = entity.Email,
            DisplayName = entity.DisplayName,
            Status = entity.Status
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<UserRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<User?> GetByEmailAsync(Guid tenantId, string email, CancellationToken cancellationToken = default)
    {
        var row = await Context.Set<UserRow>()
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.Email == email, cancellationToken);
        return row != null ? MapToDomain(row) : null;
    }

    public async Task<IReadOnlyList<User>> GetByTenantIdAsync(Guid tenantId,CancellationToken cancellationToken = default)
    {
        var rows = await Context.Set<UserRow>()
            .Where(r => r.TenantId == tenantId)
            .ToListAsync(cancellationToken);
        return rows.Select(MapToDomain).ToList();
    }


    public Task<bool> EmailExistsAsync(Guid tenantId, string email, CancellationToken cancellationToken = default)
    {
        return Context.Set<UserRow>()
            .AnyAsync(r => r.TenantId == tenantId && r.Email == email, cancellationToken);
    }
 

    public async Task<IReadOnlyList<User>> GetByStatusAsync(Guid tenantId, string status, CancellationToken cancellationToken = default)
    {
        var rows = await Context.Set<UserRow>()
            .Where(r => r.TenantId == tenantId && r.Status == status)
            .ToListAsync(cancellationToken);
        return rows.Select(MapToDomain).ToList();
    }
 
}
