using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Content.Domain.Core;
using TechWayFit.ContentOS.Content.Ports.Core;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Content;

public class ContentVersionRepository : EfCoreRepository<ContentVersion, ContentVersionRow, Guid>, IContentVersionRepository
{
    public ContentVersionRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override ContentVersion MapToDomain(ContentVersionRow row)
    {
        return new ContentVersion
        {
            Id = row.Id,
            TenantId = row.TenantId,
            ContentItemId = row.ContentItemId,
            VersionNumber = row.VersionNumber,
            Lifecycle = row.Lifecycle,
            WorkflowStateId = row.WorkflowStateId,
            PublishedAt = row.PublishedAt,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override ContentVersionRow MapToRow(ContentVersion entity)
    {
        var row = new ContentVersionRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            ContentItemId = entity.ContentItemId,
            VersionNumber = entity.VersionNumber,
            Lifecycle = entity.Lifecycle,
            WorkflowStateId = entity.WorkflowStateId,
            PublishedAt = entity.PublishedAt
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<ContentVersionRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<IEnumerable<ContentVersion>> GetByItemAsync(Guid tenantId, Guid contentItemId)
    {
        var rows = await DbSet
            .Where(r => r.TenantId == tenantId && r.ContentItemId == contentItemId)
            .OrderByDescending(r => r.VersionNumber)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task<ContentVersion?> GetPublishedAsync(Guid tenantId, Guid contentItemId)
    {
        var row = await DbSet
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.ContentItemId == contentItemId && r.Lifecycle == "published");
        return row != null ? MapToDomain(row) : null;
    }

    public async Task<ContentVersion?> GetLatestDraftAsync(Guid tenantId, Guid contentItemId)
    {
        var row = await DbSet
            .Where(r => r.TenantId == tenantId && r.ContentItemId == contentItemId && r.Lifecycle == "draft")
            .OrderByDescending(r => r.VersionNumber)
            .FirstOrDefaultAsync();
        return row != null ? MapToDomain(row) : null;
    }

    public async Task PublishAsync(Guid versionId, DateTime? publishedAt = null)
    {
        var row = await DbSet.FindAsync(versionId);
        if (row != null)
        {
            row.Lifecycle = "published";
            row.PublishedAt = publishedAt ?? DateTime.UtcNow;
            row.UpdatedOn = DateTime.UtcNow;
        }
    }

    public async Task ArchiveAsync(Guid versionId)
    {
        var row = await DbSet.FindAsync(versionId);
        if (row != null)
        {
            row.Lifecycle = "archived";
            row.UpdatedOn = DateTime.UtcNow;
        }
    }
}
