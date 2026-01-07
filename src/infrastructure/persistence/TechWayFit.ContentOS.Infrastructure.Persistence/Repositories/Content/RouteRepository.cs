using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Content.Domain.Hierarchy;
using TechWayFit.ContentOS.Content.Ports.Hierarchy;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Content;

public class RouteRepository : EfCoreRepository<Route, RouteRow, Guid>, IRouteRepository
{
    public RouteRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override Route MapToDomain(RouteRow row)
    {
        return new Route
        {
            Id = row.Id,
            TenantId = row.TenantId,
            SiteId = row.SiteId,
            NodeId = row.NodeId,
            RoutePath = row.RoutePath,
            IsPrimary = row.IsPrimary,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override RouteRow MapToRow(Route entity)
    {
        var row = new RouteRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            SiteId = entity.SiteId,
            NodeId = entity.NodeId,
            RoutePath = entity.RoutePath,
            IsPrimary = entity.IsPrimary
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<RouteRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<IEnumerable<Route>> GetByNodeAsync(Guid tenantId, Guid nodeId)
    {
        var rows = await DbSet
            .Where(r => r.TenantId == tenantId && r.NodeId == nodeId)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task<Route?> GetByRoutePathAsync(Guid tenantId, Guid siteId, string routePath)
    {
        var row = await DbSet
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.SiteId == siteId && r.RoutePath == routePath);
        return row != null ? MapToDomain(row) : null;
    }

    public async Task<Route?> GetPrimaryAsync(Guid tenantId, Guid nodeId)
    {
        var row = await DbSet
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.NodeId == nodeId && r.IsPrimary);
        return row != null ? MapToDomain(row) : null;
    }
}
