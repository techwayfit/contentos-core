using TechWayFit.ContentOS.Content.Domain.Core;
using TechWayFit.ContentOS.Content.Ports.Core;

namespace TechWayFit.ContentOS.Content.Application.ContentItems;

/// <summary>
/// Use case: Get a content item by ID (with optional version info)
/// </summary>
public sealed class GetContentItemUseCase
{
    private readonly IContentItemRepository _contentItemRepository;
    private readonly IContentVersionRepository _versionRepository;

    public GetContentItemUseCase(
        IContentItemRepository contentItemRepository,
        IContentVersionRepository versionRepository)
    {
        _contentItemRepository = contentItemRepository;
        _versionRepository = versionRepository;
    }

    public async Task<ContentItem?> ExecuteAsync(
        Guid tenantId,
 Guid contentItemId,
        CancellationToken cancellationToken = default)
    {
      var contentItem = await _contentItemRepository.GetByIdAsync(contentItemId, cancellationToken);

 // Validate tenant ownership
     if (contentItem != null && contentItem.TenantId != tenantId)
        {
        return null;
        }

        return contentItem;
    }

    public async Task<(ContentItem? Item, ContentVersion? LatestVersion)> ExecuteWithVersionAsync(
        Guid tenantId,
        Guid contentItemId,
        CancellationToken cancellationToken = default)
    {
        var contentItem = await ExecuteAsync(tenantId, contentItemId, cancellationToken);
        if (contentItem == null)
        {
            return (null, null);
      }

        var versions = await _versionRepository.GetByItemAsync(tenantId, contentItemId);
        var latestVersion = versions.OrderByDescending(v => v.VersionNumber).FirstOrDefault();

        return (contentItem, latestVersion);
    }
}
