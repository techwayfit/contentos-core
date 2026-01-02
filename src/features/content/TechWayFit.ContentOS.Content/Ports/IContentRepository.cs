using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Domain;

namespace TechWayFit.ContentOS.Content.Ports;

/// <summary>
/// Repository port for content item persistence
/// Implementation in Infrastructure layer
/// </summary>
public interface IContentRepository
{
    Task<ContentItem?> GetByIdAsync(ContentItemId id, CancellationToken cancellationToken = default);
    Task<PagedResult<ContentItem>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(ContentItem content, CancellationToken cancellationToken = default);
    Task UpdateAsync(ContentItem content, CancellationToken cancellationToken = default);
    Task DeleteAsync(ContentItemId id, CancellationToken cancellationToken = default);
}
