using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Mappers;
using TechWayFit.ContentOS.Tenancy.Domain;
using TechWayFit.ContentOS.Tenancy.Ports;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Repositories;

/// <summary>
/// EF Core implementation of ITenantRepository
/// </summary>
public sealed class TenantRepository : ITenantRepository
{
    private readonly ContentOsDbContext _context;

    public TenantRepository(ContentOsDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        var row = TenantMapper.ToRow(tenant);
        await _context.Set<TenantRow>().AddAsync(row, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return row.Id;
    }

    public async Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var row = await _context.Set<TenantRow>()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        return row == null ? null : TenantMapper.ToDomain(row);
    }

    public async Task<Tenant?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        var row = await _context.Set<TenantRow>()
            .FirstOrDefaultAsync(t => t.Key == key, cancellationToken);

        return row == null ? null : TenantMapper.ToDomain(row);
    }

    public async Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        var row = await _context.Set<TenantRow>()
            .FirstOrDefaultAsync(t => t.Id == tenant.Id, cancellationToken);

        if (row == null)
            throw new InvalidOperationException($"Tenant {tenant.Id} not found");

        row.Name = tenant.Name;
        row.Status = (int)tenant.Status;
        row.UpdatedAt = tenant.UpdatedAt;

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Tenant>> ListAsync(TenantStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<TenantRow>().AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == (int)status.Value);
        }

        var rows = await query
            .OrderBy(t => t.CreatedAt)
            .ToListAsync(cancellationToken);

        return rows.Select(TenantMapper.ToDomain).ToList();
    }

    public async Task<bool> KeyExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TenantRow>()
            .AnyAsync(t => t.Key == key, cancellationToken);
    }
}
