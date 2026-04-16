using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Ports.Core;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application.ContentVersions;

/// <summary>
/// Use case: Publish a content version
/// </summary>
public sealed class PublishContentVersionUseCase
{
    private readonly IContentVersionRepository _versionRepository;
    private readonly IContentItemRepository _contentItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PublishContentVersionUseCase(
 IContentVersionRepository versionRepository,
IContentItemRepository contentItemRepository,
     IUnitOfWork unitOfWork)
    {
   _versionRepository = versionRepository;
        _contentItemRepository = contentItemRepository;
        _unitOfWork = unitOfWork;
  }

    public async Task<Result<bool, string>> ExecuteAsync(
Guid tenantId,
Guid versionId,
   CancellationToken cancellationToken = default)
{
        // Get version
  var version = await _versionRepository.GetByIdAsync(versionId, cancellationToken);
        if (version == null || version.TenantId != tenantId)
        {
        return Result.Fail<bool, string>($"Version with ID '{versionId}' not found");
        }

   // Unpublish current published version if exists
 var currentPublished = await _versionRepository.GetPublishedAsync(tenantId, version.ContentItemId);
    if (currentPublished != null && currentPublished.Id != versionId)
   {
       await _versionRepository.ArchiveAsync(currentPublished.Id);
    }

   // Publish this version
await _versionRepository.PublishAsync(versionId, DateTime.UtcNow);

    // Update content item status
        var contentItem = await _contentItemRepository.GetByIdAsync(version.ContentItemId, cancellationToken);
  if (contentItem != null)
        {
     contentItem.Status = "published";
     contentItem.Audit.UpdatedOn = DateTime.UtcNow;
  await _contentItemRepository.UpdateAsync(contentItem, cancellationToken);
        }

  await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish ContentVersionPublished event

   return Result.Ok<bool, string>(true);
    }
}
