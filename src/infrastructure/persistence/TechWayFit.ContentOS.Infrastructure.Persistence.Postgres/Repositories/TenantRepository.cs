using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;
using TechWayFit.ContentOS.Tenancy.Domain;
using TechWayFit.ContentOS.Tenancy.Ports;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Repositories;

/// <summary>
/// EF Core repository implementation for Tenant entity
/// </summary>
public sealed class TenantRepository : EfCoreRepository<Tenant, TenantRow, Guid>, ITenantRepository
{
    public TenantRepository(ContentOsDbContext context) : base(context)
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
        // Use reflection to create Tenant since it has a private constructor
        var tenant = Activator.CreateInstance(typeof(Tenant), nonPublic: true) as Tenant;
        if (tenant == null) throw new InvalidOperationException("Failed to create Tenant instance");

        // Set properties using reflection
        var idProp = typeof(Tenant).GetProperty(nameof(Tenant.Id));
        var keyProp = typeof(Tenant).GetProperty(nameof(Tenant.Key));
        var nameProp = typeof(Tenant).GetProperty(nameof(Tenant.Name));
        var statusProp = typeof(Tenant).GetProperty(nameof(Tenant.Status));
        var createdAtProp = typeof(Tenant).GetProperty(nameof(Tenant.CreatedAt));
        var updatedAtProp = typeof(Tenant).GetProperty(nameof(Tenant.UpdatedAt));

        idProp?.SetValue(tenant, row.Id);
        keyProp?.SetValue(tenant, row.Key);
        nameProp?.SetValue(tenant, row.Name);
        statusProp?.SetValue(tenant, (TenantStatus)row.Status);
        createdAtProp?.SetValue(tenant, row.CreatedAt);
        updatedAtProp?.SetValue(tenant, row.UpdatedAt);

        return tenant;
    }

    protected override Guid GetRowId(TenantRow row) => row.Id;

    protected override Expression<Func<TenantRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    // ==================== SPECIFIC METHODS ====================

    public async Task<Tenant?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        var row = await Context.Set<TenantRow>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Key == key, cancellationToken);

        return row == null ? null : MapToDomain(row);
    }

    public async Task<bool> KeyExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TenantRow>()
            .AnyAsync(t => t.Key == key, cancellationToken);
    }

    public async Task<IReadOnlyList<Tenant>> ListByStatusAsync(TenantStatus status, CancellationToken cancellationToken = default)
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
