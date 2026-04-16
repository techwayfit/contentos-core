using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Ports.Core;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application.ContentTypes;

/// <summary>
/// Use case: Update an existing content type
/// </summary>
public sealed class UpdateContentTypeUseCase
{
    private readonly IContentTypeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContentTypeUseCase(
   IContentTypeRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool, string>> ExecuteAsync(
     Guid tenantId,
        Guid id,
        string displayName,
  CancellationToken cancellationToken = default)
    {
   // Validate inputs
        if (string.IsNullOrWhiteSpace(displayName))
   return Result.Fail<bool, string>("Display name cannot be empty");

     // Get existing content type
        var contentType = await _repository.GetByIdAsync(id, cancellationToken);
        if (contentType == null || contentType.TenantId != tenantId)
        {
        return Result.Fail<bool, string>($"Content type with ID '{id}' not found");
     }

        // Update fields
        contentType.DisplayName = displayName;
        contentType.Audit.UpdatedOn = DateTime.UtcNow;

        // Persist
     await _repository.UpdateAsync(contentType, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish ContentTypeUpdated domain event

        return Result.Ok<bool, string>(true);
    }
}
