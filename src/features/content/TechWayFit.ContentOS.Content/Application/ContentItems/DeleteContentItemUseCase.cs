using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Ports.Core;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application.ContentItems;

/// <summary>
/// Use case: Delete a content item
/// </summary>
public sealed class DeleteContentItemUseCase
{
  private readonly IContentItemRepository _contentItemRepository;
    private readonly IContentVersionRepository _versionRepository;
    private readonly IContentFieldValueRepository _fieldValueRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteContentItemUseCase(
    IContentItemRepository contentItemRepository,
        IContentVersionRepository versionRepository,
        IContentFieldValueRepository fieldValueRepository,
        IUnitOfWork unitOfWork)
    {
        _contentItemRepository = contentItemRepository;
        _versionRepository = versionRepository;
        _fieldValueRepository = fieldValueRepository;
    _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool, string>> ExecuteAsync(
   Guid tenantId,
      Guid contentItemId,
        CancellationToken cancellationToken = default)
    {
        // Get existing content item
 var contentItem = await _contentItemRepository.GetByIdAsync(contentItemId, cancellationToken);
        if (contentItem == null || contentItem.TenantId != tenantId)
        {
        return Result.Fail<bool, string>($"Content item with ID '{contentItemId}' not found");
        }

        // Get all versions
        var versions = await _versionRepository.GetByItemAsync(tenantId, contentItemId);

      // Delete field values for all versions
        foreach (var version in versions)
  {
       var fieldValues = await _fieldValueRepository.GetByVersionAsync(tenantId, version.Id);
  foreach (var fieldValue in fieldValues)
            {
      await _fieldValueRepository.DeleteAsync(fieldValue.Id, cancellationToken);
    }
    }

   // Delete versions
        foreach (var version in versions)
      {
 await _versionRepository.DeleteAsync(version.Id, cancellationToken);
        }

 // Delete content item
    await _contentItemRepository.DeleteAsync(contentItemId, cancellationToken);
     await _unitOfWork.SaveChangesAsync(cancellationToken);

  // TODO: Publish ContentItemDeleted event
        // TODO: Delete associated content nodes

        return Result.Ok<bool, string>(true);
    }
}
