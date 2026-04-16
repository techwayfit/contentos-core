using TechWayFit.ContentOS.Content.Domain.Core;
using TechWayFit.ContentOS.Content.Ports.Core;

namespace TechWayFit.ContentOS.Content.Application.ContentVersions;

/// <summary>
/// Use case: Get a specific version of content
/// </summary>
public sealed class GetContentVersionUseCase
{
    private readonly IContentVersionRepository _versionRepository;

    public GetContentVersionUseCase(IContentVersionRepository versionRepository)
    {
        _versionRepository = versionRepository;
}

    public async Task<ContentVersion?> ExecuteAsync(
  Guid tenantId,
        Guid versionId,
        CancellationToken cancellationToken = default)
  {
        var version = await _versionRepository.GetByIdAsync(versionId, cancellationToken);

  // Validate tenant ownership
        if (version != null && version.TenantId != tenantId)
      {
       return null;
}

        return version;
    }
}
