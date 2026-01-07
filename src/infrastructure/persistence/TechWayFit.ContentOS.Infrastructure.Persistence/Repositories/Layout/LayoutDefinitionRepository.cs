using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Content.Domain.Layout;
using TechWayFit.ContentOS.Content.Ports.Layout;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Layout;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Layout;

public class LayoutDefinitionRepository : EfCoreRepository<LayoutDefinition, LayoutDefinitionRow, Guid>, ILayoutDefinitionRepository
{
    public LayoutDefinitionRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override LayoutDefinition MapToDomain(LayoutDefinitionRow row)
    {
        return new LayoutDefinition
        {
            Id = row.Id,
            TenantId = row.TenantId,
            LayoutKey = row.LayoutKey,
            DisplayName = row.DisplayName,
            RegionsRulesJson = row.RegionsRulesJson,
            Version = row.Version,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override LayoutDefinitionRow MapToRow(LayoutDefinition entity)
    {
        var row = new LayoutDefinitionRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            LayoutKey = entity.LayoutKey,
            DisplayName = entity.DisplayName,
            RegionsRulesJson = entity.RegionsRulesJson,
            Version = entity.Version
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<LayoutDefinitionRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<LayoutDefinition?> GetByKeyAsync(Guid tenantId, string layoutKey, int? version = null)
    {
        var query = DbSet.Where(r => r.TenantId == tenantId && r.LayoutKey == layoutKey);
        
        if (version.HasValue)
        {
            query = query.Where(r => r.Version == version.Value);
        }
        else
        {
            query = query.OrderByDescending(r => r.Version);
        }
        
        var row = await query.FirstOrDefaultAsync();
        return row != null ? MapToDomain(row) : null;
    }

    public async Task<IEnumerable<LayoutDefinition>> GetByTenantAsync(Guid tenantId)
    {
        var rows = await DbSet
            .Where(r => r.TenantId == tenantId)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }
}
