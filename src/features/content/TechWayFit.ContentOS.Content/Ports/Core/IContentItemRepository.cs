using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Content.Domain.Core;

namespace TechWayFit.ContentOS.Content.Ports.Core;

/// <summary>
/// Repository interface for ContentItem entity persistence
/// </summary>
public interface IContentItemRepository : IRepository<ContentItem, Guid>
{
    Task<IReadOnlyList<ContentItem>> GetByContentTypeIdAsync(Guid tenantId, Guid contentTypeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContentItem>> GetBySiteIdAsync(Guid tenantId, Guid siteId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContentItem>> GetByStatusAsync(Guid tenantId, string status, CancellationToken cancellationToken = default);
}
