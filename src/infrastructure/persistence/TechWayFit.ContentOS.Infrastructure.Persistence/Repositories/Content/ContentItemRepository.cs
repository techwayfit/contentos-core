using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Content.Domain.Core;
using TechWayFit.ContentOS.Content.Ports.Core;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Content;

public class ContentItemRepository : EfCoreRepository<ContentItem, ContentItemRow, Guid>, IContentItemRepository
{
    public ContentItemRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override ContentItem MapToDomain(ContentItemRow row)
    {
        return new ContentItem
        {
            Id = row.Id,
            TenantId = row.TenantId,
            SiteId = row.SiteId,
            ContentTypeId = row.ContentTypeId,
            Status = row.Status,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override ContentItemRow MapToRow(ContentItem entity)
    {
        var row = new ContentItemRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            SiteId = entity.SiteId,
            ContentTypeId = entity.ContentTypeId,
            Status = entity.Status
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<ContentItemRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<IEnumerable<ContentItem>> GetBySiteAsync(Guid tenantId, Guid siteId)
    {
        var rows = await DbSet
            .Where(r => r.TenantId == tenantId && r.SiteId == siteId)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task<IEnumerable<ContentItem>> GetByTypeAsync(Guid tenantId, Guid contentTypeId)
    {
        var rows = await DbSet
            .Where(r => r.TenantId == tenantId && r.ContentTypeId == contentTypeId)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task ArchiveAsync(Guid itemId)
    {
        var row = await DbSet.FindAsync(itemId);
        if (row != null)
        {
            row.Status = "archived";
            row.UpdatedOn = DateTime.UtcNow;
        }
    }
}
