using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Content.Domain.Hierarchy;

namespace TechWayFit.ContentOS.Content.Ports.Hierarchy;

/// <summary>
/// Repository interface for ContentNode entity persistence
/// </summary>
public interface IContentNodeRepository : IRepository<ContentNode, Guid>
{
    Task<IReadOnlyList<ContentNode>> GetChildrenAsync(Guid tenantId, Guid? parentId, CancellationToken cancellationToken = default);
    Task<ContentNode?> GetBySlugAsync(Guid tenantId, Guid siteId, Guid? parentId, string slug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContentNode>> GetBySiteIdAsync(Guid tenantId, Guid siteId, CancellationToken cancellationToken = default);
}
