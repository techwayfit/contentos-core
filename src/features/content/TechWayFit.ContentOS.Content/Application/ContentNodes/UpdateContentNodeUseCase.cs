using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Ports.Hierarchy;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application.ContentNodes;

/// <summary>
/// Use case: Update an existing content node
/// </summary>
public sealed class UpdateContentNodeUseCase
{
    private readonly IContentNodeRepository _nodeRepository;
    private readonly IUnitOfWork _unitOfWork;

public UpdateContentNodeUseCase(
    IContentNodeRepository nodeRepository,
    IUnitOfWork unitOfWork)
{
        _nodeRepository = nodeRepository;
    _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool, string>> ExecuteAsync(
     Guid tenantId,
        Guid nodeId,
        string slug,
    Guid? contentItemId = null,
      CancellationToken cancellationToken = default)
    {
 // Validate inputs
   if (string.IsNullOrWhiteSpace(slug))
   return Result.Fail<bool, string>("Slug cannot be empty");

        // Validate slug format
     if (!System.Text.RegularExpressions.Regex.IsMatch(slug, "^[a-z0-9-]+$"))
     return Result.Fail<bool, string>("Slug must be lowercase alphanumeric with hyphens only");

    // Get existing node
        var node = await _nodeRepository.GetByIdAsync(nodeId, cancellationToken);
    if (node == null || node.TenantId != tenantId)
        {
    return Result.Fail<bool, string>($"Node with ID '{nodeId}' not found");
        }

    // Check if new slug conflicts with siblings
  if (node.Slug != slug)
        {
     var existingNode = await _nodeRepository.GetBySlugAsync(
       tenantId, node.SiteId, node.ParentId, slug, cancellationToken);
            if (existingNode != null && existingNode.Id != nodeId)
         {
       return Result.Fail<bool, string>($"Node with slug '{slug}' already exists under this parent");
            }
    }

   // Update fields
     node.Slug = slug;
   if (contentItemId.HasValue)
        {
         node.ContentItemId = contentItemId;
        }
        node.Audit.UpdatedOn = DateTime.UtcNow;

        // Persist
     await _nodeRepository.UpdateAsync(node, cancellationToken);
     await _unitOfWork.SaveChangesAsync(cancellationToken);

 // TODO: Publish ContentNodeUpdated event

  return Result.Ok<bool, string>(true);
    }
}
