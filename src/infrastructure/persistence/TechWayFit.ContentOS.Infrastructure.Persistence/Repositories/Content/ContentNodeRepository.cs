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
        return new ContentNode
        {
            Id = row.Id,
            TenantId = row.TenantId,
            SiteId = row.SiteId,
            ParentId = row.ParentId,
            NodeType = row.NodeType,
            ContentItemId = row.ContentItemId,
            Title = row.Title,
            Path = row.Path,
            SortOrder = row.SortOrder,
            InheritAcl = row.InheritAcl,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override ContentNodeRow MapToRow(ContentNode entity)
    {
        var row = new ContentNodeRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            SiteId = entity.SiteId,
            ParentId = entity.ParentId,
            NodeType = entity.NodeType,
            ContentItemId = entity.ContentItemId,
            Title = entity.Title,
            Path = entity.Path,
            SortOrder = entity.SortOrder,
            InheritAcl = entity.InheritAcl
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<ContentNodeRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<ContentNode?> GetByPathAsync(Guid tenantId, Guid siteId, string path)
    {
        var row = await DbSet
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.SiteId == siteId && r.Path == path);
        return row != null ? MapToDomain(row) : null;
    }

    public async Task<IEnumerable<ContentNode>> GetChildrenAsync(Guid tenantId, Guid? parentId)
    {
        var rows = await DbSet
            .Where(r => r.TenantId == tenantId && r.ParentId == parentId)
            .OrderBy(r => r.SortOrder)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task<IEnumerable<ContentNode>> GetTreeAsync(Guid tenantId, Guid siteId, Guid? rootNodeId = null)
    {
        // Simple implementation - get all nodes under root
        // For production, consider using recursive CTE or ltree
        var query = DbSet.Where(r => r.TenantId == tenantId && r.SiteId == siteId);
        
        if (rootNodeId.HasValue)
        {
            var rootNode = await DbSet.FindAsync(rootNodeId.Value);
            if (rootNode != null)
            {
                query = query.Where(r => r.Path.StartsWith(rootNode.Path));
            }
        }
        
        var rows = await query.OrderBy(r => r.Path).ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task MoveAsync(Guid nodeId, Guid? newParentId, int sortOrder)
    {
        var row = await DbSet.FindAsync(nodeId);
        if (row != null)
        {
            row.ParentId = newParentId;
            row.SortOrder = sortOrder;
            row.UpdatedOn = DateTime.UtcNow;
            
            // In production, update path for all descendants recursively
            // This is a simplified version
        }
    }
}
