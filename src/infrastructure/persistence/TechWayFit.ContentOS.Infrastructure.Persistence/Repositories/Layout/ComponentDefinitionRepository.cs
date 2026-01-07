using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Content.Domain.Layout;
using TechWayFit.ContentOS.Content.Ports.Layout;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Layout;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Layout;

public class ComponentDefinitionRepository : EfCoreRepository<ComponentDefinition, ComponentDefinitionRow, Guid>, IComponentDefinitionRepository
{
    public ComponentDefinitionRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override ComponentDefinition MapToDomain(ComponentDefinitionRow row)
    {
        return new ComponentDefinition
        {
            Id = row.Id,
            TenantId = row.TenantId,
            ComponentKey = row.ComponentKey,
            DisplayName = row.DisplayName,
            PropsSchemaJson = row.PropsSchemaJson,
            OwnerModule = row.OwnerModule,
            Version = row.Version,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override ComponentDefinitionRow MapToRow(ComponentDefinition entity)
    {
        var row = new ComponentDefinitionRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            ComponentKey = entity.ComponentKey,
            DisplayName = entity.DisplayName,
            PropsSchemaJson = entity.PropsSchemaJson,
            OwnerModule = entity.OwnerModule,
            Version = entity.Version
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<ComponentDefinitionRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<ComponentDefinition?> GetByKeyAsync(Guid tenantId, string componentKey, int? version = null)
    {
        var query = DbSet.Where(r => r.TenantId == tenantId && r.ComponentKey == componentKey);
        
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

    public async Task<IEnumerable<ComponentDefinition>> GetByModuleAsync(Guid tenantId, string ownerModule)
    {
        var rows = await DbSet
            .Where(r => r.TenantId == tenantId && r.OwnerModule == ownerModule)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }
}
