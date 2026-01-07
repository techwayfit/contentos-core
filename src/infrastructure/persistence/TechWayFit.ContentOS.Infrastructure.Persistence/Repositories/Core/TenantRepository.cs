using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;
using TechWayFit.ContentOS.Infrastructure.Persistence.Repositories;
using TechWayFit.ContentOS.Tenancy.Domain;
using TechWayFit.ContentOS.Tenancy.Ports;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Core;

/// <summary>
/// EF Core repository implementation for Tenant entity
/// Provider-agnostic - uses standard EF Core features
/// </summary>
public class TenantRepository : EfCoreRepository<Tenant, TenantRow, Guid>, ITenantRepository
{
    public TenantRepository(DbContext context) : base(context)
    {
    }

    protected override TenantRow MapToRow(Tenant entity)
    {
        return new TenantRow
        {
            Id = entity.Id,
            Key = entity.Key,
            Name = entity.Name,
            Status = (int)entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    protected override Tenant MapToDomain(TenantRow row)
    {
        return new Tenant
        {
            Id = row.Id,
            Key = row.Key,
            Name = row.Name,
            Status = (TenantStatus)row.Status,
            CreatedAt = row.CreatedAt,
            UpdatedAt = row.UpdatedAt
        };
    }

    protected override Guid GetRowId(TenantRow row) => row.Id;

    protected override Expression<Func<TenantRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    // ==================== SPECIFIC METHODS ====================

    public virtual async Task<Tenant?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        var row = await Context.Set<TenantRow>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Key == key, cancellationToken);

        return row == null ? null : MapToDomain(row);
    }

    public virtual async Task<bool> KeyExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TenantRow>()
            .AnyAsync(t => t.Key == key, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<Tenant>> ListByStatusAsync(TenantStatus status, CancellationToken cancellationToken = default)
    {
        var statusInt = (int)status;
        var rows = await Context.Set<TenantRow>()
            .AsNoTracking()
            .Where(t => t.Status == statusInt)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

        return rows.Select(MapToDomain).ToList();
    }
}
