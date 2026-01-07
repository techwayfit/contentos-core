using TechWayFit.ContentOS.Abstractions.Repositories;

namespace TechWayFit.ContentOS.Content.Ports.Core;

public interface IContentVersionRepository : IRepository<Domain.Core.ContentVersion, Guid>
{
    Task<IEnumerable<Domain.Core.ContentVersion>> GetByItemAsync(Guid tenantId, Guid contentItemId);
    Task<Domain.Core.ContentVersion?> GetPublishedAsync(Guid tenantId, Guid contentItemId);
    Task<Domain.Core.ContentVersion?> GetLatestDraftAsync(Guid tenantId, Guid contentItemId);
    Task PublishAsync(Guid versionId, DateTime? publishedAt = null);
    Task ArchiveAsync(Guid versionId);
}
