using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;
using TechWayFit.ContentOS.Tenancy.Domain.Core;
using TechWayFit.ContentOS.Tenancy.Ports.Core;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Core;

public class SiteRepository : EfCoreRepository<Site, SiteRow, Guid>, ISiteRepository
{
    public SiteRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override Site MapToDomain(SiteRow row)
    {
        return new Site
        {
            Id = row.Id,
            TenantId = row.TenantId,
            Name = row.Name,
            HostName = row.HostName,
            DefaultLocale = row.DefaultLocale,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override SiteRow MapToRow(Site entity)
    {
        var row = new SiteRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            Name = entity.Name,
            HostName = entity.HostName,
            DefaultLocale = entity.DefaultLocale
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<SiteRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }
 


    public async Task<Site?> GetByHostNameAsync(Guid tenantId, string hostName, CancellationToken cancellationToken = default)
    {
          var row = await Context.Set<SiteRow>()
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.HostName == hostName);
        return row != null ? MapToDomain(row) : null;
    }

    public Task<bool> HostNameExistsAsync(Guid tenantId, string hostName, CancellationToken cancellationToken = default)
    {
        return Context.Set<SiteRow>()
            .AnyAsync(r => r.TenantId == tenantId && r.HostName == hostName, cancellationToken);
    }

    public async Task<IReadOnlyList<Site>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var rows = await Context.Set<SiteRow>()
            .Where(r => r.TenantId == tenantId)
            .ToListAsync(cancellationToken);
        return rows.Select(MapToDomain).ToList();
    }
}
