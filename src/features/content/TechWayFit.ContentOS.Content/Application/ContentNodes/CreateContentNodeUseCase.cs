using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Domain.Hierarchy;
using TechWayFit.ContentOS.Content.Ports.Hierarchy;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application.ContentNodes;

/// <summary>
/// Use case: Create a new content node in the hierarchy
/// </summary>
public sealed class CreateContentNodeUseCase
{
    private readonly IContentNodeRepository _nodeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateContentNodeUseCase(
        IContentNodeRepository nodeRepository,
     IUnitOfWork unitOfWork)
    {
        _nodeRepository = nodeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, string>> ExecuteAsync(
    Guid tenantId,
        Guid siteId,
  Guid? parentId,
        Guid? contentItemId,
        string slug,
        int? sortOrder = null,
        CancellationToken cancellationToken = default)
    {
    // Validate inputs
        if (string.IsNullOrWhiteSpace(slug))
    return Result.Fail<Guid, string>("Slug cannot be empty");

        // Validate slug format
        if (!System.Text.RegularExpressions.Regex.IsMatch(slug, "^[a-z0-9-]+$"))
     return Result.Fail<Guid, string>("Slug must be lowercase alphanumeric with hyphens only");

        // Check if slug already exists under same parent
        var existingNode = await _nodeRepository.GetBySlugAsync(tenantId, siteId, parentId, slug, cancellationToken);
        if (existingNode != null)
        {
  return Result.Fail<Guid, string>($"Node with slug '{slug}' already exists under this parent");
        }

// Determine sort order
        int finalSortOrder = sortOrder ?? 0;
        if (!sortOrder.HasValue)
      {
            var siblings = await _nodeRepository.GetChildrenAsync(tenantId, parentId, cancellationToken);
     finalSortOrder = siblings.Any() ? siblings.Max(n => n.SortOrder) + 1 : 0;
   }

     // Create node
   var node = new ContentNode
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            SiteId = siteId,
            ParentId = parentId,
   ContentItemId = contentItemId,
     Slug = slug,
            SortOrder = finalSortOrder,
            Audit = new AuditInfo
     {
                CreatedOn = DateTime.UtcNow
     }
        };

        // Persist
    await _nodeRepository.AddAsync(node, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish ContentNodeCreated event

        return Result.Ok<Guid, string>(node.Id);
}
}
