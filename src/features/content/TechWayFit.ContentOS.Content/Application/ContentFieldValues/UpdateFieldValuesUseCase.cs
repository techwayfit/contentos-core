using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Domain.Core;
using TechWayFit.ContentOS.Content.Ports.Core;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application.ContentFieldValues;

/// <summary>
/// Use case: Update field values for a content version
/// </summary>
public sealed class UpdateFieldValuesUseCase
{
    private readonly IContentVersionRepository _versionRepository;
    private readonly IContentFieldValueRepository _fieldValueRepository;
    private readonly IContentTypeFieldRepository _typeFieldRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFieldValuesUseCase(
        IContentVersionRepository versionRepository,
        IContentFieldValueRepository fieldValueRepository,
 IContentTypeFieldRepository typeFieldRepository,
    IUnitOfWork unitOfWork)
    {
        _versionRepository = versionRepository;
        _fieldValueRepository = fieldValueRepository;
        _typeFieldRepository = typeFieldRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool, string>> ExecuteAsync(
   Guid tenantId,
        Guid versionId,
        Dictionary<string, string> fieldValues,
        string? locale = null,
        CancellationToken cancellationToken = default)
    {
        // Validate version exists
     var version = await _versionRepository.GetByIdAsync(versionId, cancellationToken);
        if (version == null || version.TenantId != tenantId)
        {
            return Result.Fail<bool, string>($"Version with ID '{versionId}' not found");
        }

    // Get existing field values
        var existingValues = await _fieldValueRepository.GetByVersionAsync(tenantId, versionId);

      foreach (var kvp in fieldValues)
        {
            var fieldKey = kvp.Key;
            var valueJson = kvp.Value;

      // Find existing field value
    var existingValue = existingValues.FirstOrDefault(fv => 
     fv.FieldKey == fieldKey && fv.Locale == locale);

  if (existingValue != null)
          {
      // Update existing
           existingValue.ValueJson = valueJson;
                existingValue.Audit.UpdatedOn = DateTime.UtcNow;
  await _fieldValueRepository.UpdateAsync(existingValue, cancellationToken);
    }
   else
       {
       // Create new
     var newFieldValue = new ContentFieldValue
       {
           Id = Guid.NewGuid(),
   TenantId = tenantId,
             ContentVersionId = versionId,
         FieldKey = fieldKey,
        Locale = locale,
         ValueJson = valueJson,
          Audit = new AuditInfo
{
       CreatedOn = DateTime.UtcNow
           }
       };
        await _fieldValueRepository.AddAsync(newFieldValue, cancellationToken);
 }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish FieldValuesUpdated event

        return Result.Ok<bool, string>(true);
    }
}
