using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Ports.Hierarchy;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application.ContentNodes;

/// <summary>
/// Use case: Delete a content node
/// </summary>
public sealed class DeleteContentNodeUseCase
{
    private readonly IContentNodeRepository _nodeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteContentNodeUseCase(
IContentNodeRepository nodeRepository,
        IUnitOfWork unitOfWork)
    {
 _nodeRepository = nodeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool, string>> ExecuteAsync(
  Guid tenantId,
 Guid nodeId,
        CancellationToken cancellationToken = default)
    {
        // Get existing node
 var node = await _nodeRepository.GetByIdAsync(nodeId, cancellationToken);
   if (node == null || node.TenantId != tenantId)
        {
return Result.Fail<bool, string>($"Node with ID '{nodeId}' not found");
 }

     // Check if node has children
   var children = await _nodeRepository.GetChildrenAsync(tenantId, nodeId, cancellationToken);
        if (children.Count > 0)
     {
     return Result.Fail<bool, string>($"Cannot delete node. It has {children.Count} child node(s)");
        }

        // Delete node
  await _nodeRepository.DeleteAsync(nodeId, cancellationToken);
   await _unitOfWork.SaveChangesAsync(cancellationToken);

// TODO: Publish ContentNodeDeleted event
        // TODO: Delete associated routes

        return Result.Ok<bool, string>(true);
    }
}
