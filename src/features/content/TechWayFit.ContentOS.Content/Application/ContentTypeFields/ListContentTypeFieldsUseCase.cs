using TechWayFit.ContentOS.Content.Domain.Core;
using TechWayFit.ContentOS.Content.Ports.Core;

namespace TechWayFit.ContentOS.Content.Application.ContentTypeFields;

/// <summary>
/// Use case: List all fields for a content type
/// </summary>
public sealed class ListContentTypeFieldsUseCase
{
    private readonly IContentTypeFieldRepository _fieldRepository;

    public ListContentTypeFieldsUseCase(IContentTypeFieldRepository fieldRepository)
    {
     _fieldRepository = fieldRepository;
    }

    public async Task<IReadOnlyList<ContentTypeField>> ExecuteAsync(
  Guid tenantId,
        Guid contentTypeId,
        CancellationToken cancellationToken = default)
    {
   var fields = await _fieldRepository.GetByContentTypeAsync(tenantId, contentTypeId);
        return fields.OrderBy(f => f.SortOrder).ToList();
    }
}
