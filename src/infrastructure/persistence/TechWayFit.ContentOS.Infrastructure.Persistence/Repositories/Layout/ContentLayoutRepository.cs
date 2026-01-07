using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Content.Domain.Layout;
using TechWayFit.ContentOS.Content.Ports.Layout;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Layout;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Layout;

public class ContentLayoutRepository : EfCoreRepository<ContentLayout, ContentLayoutRow, Guid>, IContentLayoutRepository
{
    public ContentLayoutRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override ContentLayout MapToDomain(ContentLayoutRow row)
    {
        return new ContentLayout
        {
            Id = row.Id,
            TenantId = row.TenantId,
            ContentVersionId = row.ContentVersionId,
            LayoutDefinitionId = row.LayoutDefinitionId,
            CompositionJson = row.CompositionJson,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override ContentLayoutRow MapToRow(ContentLayout entity)
    {
        var row = new ContentLayoutRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            ContentVersionId = entity.ContentVersionId,
            LayoutDefinitionId = entity.LayoutDefinitionId,
            CompositionJson = entity.CompositionJson
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<ContentLayoutRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<ContentLayout?> GetByVersionAsync(Guid tenantId, Guid contentVersionId)
    {
        var row = await DbSet
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.ContentVersionId == contentVersionId);
        return row != null ? MapToDomain(row) : null;
    }
}
