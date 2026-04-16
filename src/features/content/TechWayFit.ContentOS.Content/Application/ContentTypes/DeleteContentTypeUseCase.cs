using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Ports.Core;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application.ContentTypes;

/// <summary>
/// Use case: Delete a content type
/// </summary>
public sealed class DeleteContentTypeUseCase
{
    private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IContentItemRepository _contentItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteContentTypeUseCase(
        IContentTypeRepository contentTypeRepository,
        IContentItemRepository contentItemRepository,
        IUnitOfWork unitOfWork)
    {
        _contentTypeRepository = contentTypeRepository;
        _contentItemRepository = contentItemRepository;
        _unitOfWork = unitOfWork;
    }

  public async Task<Result<bool, string>> ExecuteAsync(
        Guid tenantId,
      Guid id,
        CancellationToken cancellationToken = default)
    {
    // Get existing content type
     var contentType = await _contentTypeRepository.GetByIdAsync(id, cancellationToken);
     if (contentType == null || contentType.TenantId != tenantId)
      {
 return Result.Fail<bool, string>($"Content type with ID '{id}' not found");
    }

 // Check if content type is in use
        var contentItems = await _contentItemRepository.GetByContentTypeIdAsync(tenantId, id, cancellationToken);
        if (contentItems.Count > 0)
        {
            return Result.Fail<bool, string>($"Cannot delete content type. It is used by {contentItems.Count} content item(s)");
        }

  // Delete content type
        await _contentTypeRepository.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish ContentTypeDeleted domain event

return Result.Ok<bool, string>(true);
    }
}
