using TechWayFit.ContentOS.Content.Domain.Core;
using TechWayFit.ContentOS.Content.Ports.Core;

namespace TechWayFit.ContentOS.Content.Application.ContentItems;

/// <summary>
/// Use case: List content items with optional filtering
/// </summary>
public sealed class ListContentItemsUseCase
{
    private readonly IContentItemRepository _contentItemRepository;

    public ListContentItemsUseCase(IContentItemRepository contentItemRepository)
    {
    _contentItemRepository = contentItemRepository;
    }

    public async Task<IReadOnlyList<ContentItem>> ExecuteAsync(
 Guid tenantId,
   Guid? siteId = null,
   Guid? contentTypeId = null,
      string? status = null,
   CancellationToken cancellationToken = default)
    {
        IEnumerable<ContentItem> items;

        if (contentTypeId.HasValue)
        {
          items = await _contentItemRepository.GetByContentTypeIdAsync(tenantId, contentTypeId.Value, cancellationToken);
        }
        else if (siteId.HasValue)
        {
     items = await _contentItemRepository.GetBySiteIdAsync(tenantId, siteId.Value, cancellationToken);
        }
        else if (!string.IsNullOrEmpty(status))
        {
    items = await _contentItemRepository.GetByStatusAsync(tenantId, status, cancellationToken);
      }
    else
   {
     // Get all for tenant - this should ideally be paginated
  items = await _contentItemRepository.FindAllAsync(ci => ci.TenantId == tenantId, cancellationToken);
     }

        // Apply additional filters
        if (siteId.HasValue && !contentTypeId.HasValue)
  {
   items = items.Where(i => i.SiteId == siteId.Value);
        }
        if (!string.IsNullOrEmpty(status))
        {
  items = items.Where(i => i.Status == status);
  }

        return items.OrderByDescending(i => i.Audit.CreatedOn).ToList();
    }
}
