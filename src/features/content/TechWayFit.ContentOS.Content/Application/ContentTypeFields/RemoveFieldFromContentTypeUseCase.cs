using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Ports.Core;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application.ContentTypeFields;

/// <summary>
/// Use case: Remove a field from a content type
/// </summary>
public sealed class RemoveFieldFromContentTypeUseCase
{
    private readonly IContentTypeFieldRepository _fieldRepository;
    private readonly IContentFieldValueRepository _fieldValueRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveFieldFromContentTypeUseCase(
  IContentTypeFieldRepository fieldRepository,
        IContentFieldValueRepository fieldValueRepository,
        IUnitOfWork unitOfWork)
    {
        _fieldRepository = fieldRepository;
        _fieldValueRepository = fieldValueRepository;
        _unitOfWork = unitOfWork;
}

    public async Task<Result<bool, string>> ExecuteAsync(
        Guid tenantId,
        Guid fieldId,
    CancellationToken cancellationToken = default)
    {
     // Get existing field
    var field = await _fieldRepository.GetByIdAsync(fieldId, cancellationToken);
        if (field == null || field.TenantId != tenantId)
        {
      return Result.Fail<bool, string>($"Field with ID '{fieldId}' not found");
        }

        // TODO: Check if field has data in content items
    // This would require querying ContentFieldValue repository
        // For now, we'll allow deletion (data will be orphaned)

        // Delete field
        await _fieldRepository.DeleteAsync(fieldId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish FieldRemovedFromContentType event
        // TODO: Archive field values

        return Result.Ok<bool, string>(true);
    }
}
