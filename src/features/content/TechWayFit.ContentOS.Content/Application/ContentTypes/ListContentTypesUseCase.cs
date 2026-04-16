using TechWayFit.ContentOS.Content.Domain.Core;
using TechWayFit.ContentOS.Content.Ports.Core;

namespace TechWayFit.ContentOS.Content.Application.ContentTypes;

/// <summary>
/// Use case: List all content types for a tenant
/// </summary>
public sealed class ListContentTypesUseCase
{
    private readonly IContentTypeRepository _repository;

    public ListContentTypesUseCase(IContentTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ContentType>> ExecuteAsync(
        Guid tenantId,
   CancellationToken cancellationToken = default)
  {
        return await _repository.GetByTenantIdAsync(tenantId, cancellationToken);
    }
}
