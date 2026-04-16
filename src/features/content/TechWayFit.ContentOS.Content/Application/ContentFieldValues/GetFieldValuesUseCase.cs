using TechWayFit.ContentOS.Content.Domain.Core;
using TechWayFit.ContentOS.Content.Ports.Core;

namespace TechWayFit.ContentOS.Content.Application.ContentFieldValues;

/// <summary>
/// Use case: Get field values for a content version
/// </summary>
public sealed class GetFieldValuesUseCase
{
    private readonly IContentFieldValueRepository _fieldValueRepository;

    public GetFieldValuesUseCase(IContentFieldValueRepository fieldValueRepository)
    {
        _fieldValueRepository = fieldValueRepository;
  }

    public async Task<IReadOnlyList<ContentFieldValue>> ExecuteAsync(
        Guid tenantId,
        Guid versionId,
  string? locale = null,
     CancellationToken cancellationToken = default)
    {
    var fieldValues = await _fieldValueRepository.GetByVersionAsync(tenantId, versionId);

      if (!string.IsNullOrEmpty(locale))
 {
     fieldValues = fieldValues.Where(fv => fv.Locale == locale);
        }

        return fieldValues.ToList();
    }
}
