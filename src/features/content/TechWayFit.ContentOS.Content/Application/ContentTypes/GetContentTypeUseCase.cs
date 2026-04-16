using TechWayFit.ContentOS.Content.Domain.Core;
using TechWayFit.ContentOS.Content.Ports.Core;

namespace TechWayFit.ContentOS.Content.Application.ContentTypes;

/// <summary>
/// Use case: Get a content type by ID
/// </summary>
public sealed class GetContentTypeUseCase
{
    private readonly IContentTypeRepository _repository;

  public GetContentTypeUseCase(IContentTypeRepository repository)
  {
        _repository = repository;
    }

    public async Task<ContentType?> ExecuteAsync(
        Guid tenantId,
        Guid id,
 CancellationToken cancellationToken = default)
    {
        var contentType = await _repository.GetByIdAsync(id, cancellationToken);

      // Validate tenant ownership
        if (contentType != null && contentType.TenantId != tenantId)
        {
     return null;
        }

        return contentType;
}
}
