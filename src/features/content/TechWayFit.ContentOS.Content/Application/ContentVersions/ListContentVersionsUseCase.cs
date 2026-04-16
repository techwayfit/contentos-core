using TechWayFit.ContentOS.Content.Domain.Core;
using TechWayFit.ContentOS.Content.Ports.Core;

namespace TechWayFit.ContentOS.Content.Application.ContentVersions;

/// <summary>
/// Use case: List all versions for a content item
/// </summary>
public sealed class ListContentVersionsUseCase
{
    private readonly IContentVersionRepository _versionRepository;

    public ListContentVersionsUseCase(IContentVersionRepository versionRepository)
    {
        _versionRepository = versionRepository;
}

    public async Task<IReadOnlyList<ContentVersion>> ExecuteAsync(
    Guid tenantId,
        Guid contentItemId,
        CancellationToken cancellationToken = default)
    {
        var versions = await _versionRepository.GetByItemAsync(tenantId, contentItemId);
        return versions.OrderByDescending(v => v.VersionNumber).ToList();
    }
}
