using TechWayFit.ContentOS.Content.Domain.Core;
using TechWayFit.ContentOS.Content.Ports.Core;

namespace TechWayFit.ContentOS.Content.Application.ContentVersions;

/// <summary>
/// Use case: Get the published version of a content item
/// </summary>
public sealed class GetPublishedVersionUseCase
{
    private readonly IContentVersionRepository _versionRepository;

    public GetPublishedVersionUseCase(IContentVersionRepository versionRepository)
    {
 _versionRepository = versionRepository;
    }

    public async Task<ContentVersion?> ExecuteAsync(
Guid tenantId,
        Guid contentItemId,
   CancellationToken cancellationToken = default)
    {
        return await _versionRepository.GetPublishedAsync(tenantId, contentItemId);
    }
}
