using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Ports.Core;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application.ContentItems;

/// <summary>
/// Use case: Update content item metadata
/// </summary>
public sealed class UpdateContentItemUseCase
{
    private readonly IContentItemRepository _contentItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContentItemUseCase(
        IContentItemRepository contentItemRepository,
        IUnitOfWork unitOfWork)
    {
        _contentItemRepository = contentItemRepository;
  _unitOfWork = unitOfWork;
    }

public async Task<Result<bool, string>> ExecuteAsync(
  Guid tenantId,
      Guid contentItemId,
     string status,
        CancellationToken cancellationToken = default)
    {
        // Get existing content item
        var contentItem = await _contentItemRepository.GetByIdAsync(contentItemId, cancellationToken);
        if (contentItem == null || contentItem.TenantId != tenantId)
        {
       return Result.Fail<bool, string>($"Content item with ID '{contentItemId}' not found");
  }

  // Update fields
        contentItem.Status = status;
        contentItem.Audit.UpdatedOn = DateTime.UtcNow;

  // Persist
        await _contentItemRepository.UpdateAsync(contentItem, cancellationToken);
  await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish ContentItemUpdated event

        return Result.Ok<bool, string>(true);
    }
}
