using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Domain.Core;
using TechWayFit.ContentOS.Content.Ports.Core;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application.ContentTypeFields;

/// <summary>
/// Use case: Add a new field to a content type
/// </summary>
public sealed class AddFieldToContentTypeUseCase
{
    private readonly IContentTypeRepository _contentTypeRepository;
    private readonly IContentTypeFieldRepository _fieldRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddFieldToContentTypeUseCase(
        IContentTypeRepository contentTypeRepository,
        IContentTypeFieldRepository fieldRepository,
        IUnitOfWork unitOfWork)
    {
        _contentTypeRepository = contentTypeRepository;
        _fieldRepository = fieldRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, string>> ExecuteAsync(
        Guid tenantId,
Guid contentTypeId,
        string fieldKey,
        string dataType,
        bool isRequired,
        bool isLocalized,
        string? constraintsJson = null,
        CancellationToken cancellationToken = default)
  {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(fieldKey))
       return Result.Fail<Guid, string>("Field key cannot be empty");

        if (string.IsNullOrWhiteSpace(dataType))
         return Result.Fail<Guid, string>("Data type cannot be empty");

   // Validate field key format
        if (!System.Text.RegularExpressions.Regex.IsMatch(fieldKey, "^[a-z0-9_]+$"))
  return Result.Fail<Guid, string>("Field key must be lowercase alphanumeric with underscores only");

        // Validate content type exists
        var contentType = await _contentTypeRepository.GetByIdAsync(contentTypeId, cancellationToken);
    if (contentType == null || contentType.TenantId != tenantId)
        {
 return Result.Fail<Guid, string>($"Content type with ID '{contentTypeId}' not found");
        }

     // Check if field key already exists in this content type
        var existingFields = await _fieldRepository.GetByContentTypeAsync(tenantId, contentTypeId);
    if (existingFields.Any(f => f.FieldKey == fieldKey))
        {
            return Result.Fail<Guid, string>($"Field with key '{fieldKey}' already exists in this content type");
        }

        // Get current max sort order
     var maxSortOrder = existingFields.Any() ? existingFields.Max(f => f.SortOrder) : 0;

        // Create field
        var field = new ContentTypeField
        {
   Id = Guid.NewGuid(),
            TenantId = tenantId,
            ContentTypeId = contentTypeId,
      FieldKey = fieldKey,
    DataType = dataType,
 IsRequired = isRequired,
            IsLocalized = isLocalized,
            ConstraintsJson = constraintsJson ?? "{}",
            SortOrder = maxSortOrder + 1,
     Audit = new AuditInfo
            {
            CreatedOn = DateTime.UtcNow
            }
        };

     // Persist
        await _fieldRepository.AddAsync(field, cancellationToken);
  await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish FieldAddedToContentType event

        return Result.Ok<Guid, string>(field.Id);
 }
}
