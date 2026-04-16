using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Content.Domain.Hierarchy;
using TechWayFit.ContentOS.Content.Ports.Hierarchy;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Content;

public class ContentNodeRepository : EfCoreRepository<ContentNode, ContentNodeRow, Guid>, IContentNodeRepository
{
    public ContentNodeRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override ContentNode MapToDomain(ContentNodeRow row)
    {
        // Extract slug from path (last segment)
        string slug = row.Path.Contains('/')
            ? row.Path.Split('/').LastOrDefault() ?? string.Empty
            : row.Path;

        return new ContentNode
        {
            Id = row.Id,
            TenantId = row.TenantId,
            SiteId = row.SiteId,
            ParentId = row.ParentId,
            ContentItemId = row.ContentItemId,
            Slug = slug,
            SortOrder = row.SortOrder,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override ContentNodeRow MapToRow(ContentNode entity)
    {
        // For new nodes without a path, construct it from parent + slug
        // This is simplified - in production, would need to query parent or calculate properly
        string path = $"/{entity.Slug}"; // Default for root-level nodes

        var row = new ContentNodeRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            SiteId = entity.SiteId,
            ParentId = entity.ParentId,
            NodeType = "Item", // Default type, should be set by use-case
            ContentItemId = entity.ContentItemId,
            Title = entity.Slug, // Use slug as title by default
            Path = path, // Simplified path construction
            SortOrder = entity.SortOrder,
            InheritAcl = true // Default inheritance
        };

        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<ContentNodeRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<IReadOnlyList<ContentNode>> GetChildrenAsync(Guid tenantId, Guid? parentId, CancellationToken cancellationToken = default)
    {
        var rows = await Context.Set<ContentNodeRow>()
            .Where(r => r.TenantId == tenantId && r.ParentId == parentId)
            .OrderBy(r => r.SortOrder)
            .ToListAsync(cancellationToken);
        return rows.Select(MapToDomain).ToList();
    }

    public async Task<ContentNode?> GetBySlugAsync(Guid tenantId, Guid siteId, Guid? parentId, string slug, CancellationToken cancellationToken = default)
    {
        // Query by parent and path suffix (slug)
        var row = await Context.Set<ContentNodeRow>()
            .Where(r => r.TenantId == tenantId && r.SiteId == siteId && r.ParentId == parentId)
            .Where(r => r.Path.EndsWith("/" + slug) || r.Path == "/" + slug)
            .FirstOrDefaultAsync(cancellationToken);
        return row != null ? MapToDomain(row) : null;
    }

    public async Task<IReadOnlyList<ContentNode>> GetBySiteIdAsync(Guid tenantId, Guid siteId, CancellationToken cancellationToken = default)
    {
        var rows = await Context.Set<ContentNodeRow>()
            .Where(r => r.TenantId == tenantId && r.SiteId == siteId)
            .OrderBy(r => r.Path)
            .ToListAsync(cancellationToken);
        return rows.Select(MapToDomain).ToList();
    }
}
