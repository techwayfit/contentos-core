using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Ports.Core;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application.ContentTypeFields;

/// <summary>
/// Use case: Update an existing content type field
/// </summary>
public sealed class UpdateContentTypeFieldUseCase
{
    private readonly IContentTypeFieldRepository _fieldRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContentTypeFieldUseCase(
        IContentTypeFieldRepository fieldRepository,
        IUnitOfWork unitOfWork)
    {
        _fieldRepository = fieldRepository;
        _unitOfWork = unitOfWork;
    }

  public async Task<Result<bool, string>> ExecuteAsync(
   Guid tenantId,
        Guid fieldId,
        bool isRequired,
        bool isLocalized,
   string? constraintsJson = null,
    CancellationToken cancellationToken = default)
  {
        // Get existing field
     var field = await _fieldRepository.GetByIdAsync(fieldId, cancellationToken);
     if (field == null || field.TenantId != tenantId)
 {
    return Result.Fail<bool, string>($"Field with ID '{fieldId}' not found");
    }

        // Update fields
   field.IsRequired = isRequired;
     field.IsLocalized = isLocalized;
     if (constraintsJson != null)
  {
     field.ConstraintsJson = constraintsJson;
        }
        field.Audit.UpdatedOn = DateTime.UtcNow;

    // Persist
    await _fieldRepository.UpdateAsync(field, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

 // TODO: Publish ContentTypeFieldUpdated event

        return Result.Ok<bool, string>(true);
  }
}
