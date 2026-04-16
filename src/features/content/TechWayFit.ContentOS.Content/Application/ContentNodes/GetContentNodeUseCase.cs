using TechWayFit.ContentOS.Content.Domain.Hierarchy;
using TechWayFit.ContentOS.Content.Ports.Hierarchy;

namespace TechWayFit.ContentOS.Content.Application.ContentNodes;

/// <summary>
/// Use case: Get a content node by ID
/// </summary>
public sealed class GetContentNodeUseCase
{
    private readonly IContentNodeRepository _nodeRepository;

public GetContentNodeUseCase(IContentNodeRepository nodeRepository)
    {
     _nodeRepository = nodeRepository;
    }

    public async Task<ContentNode?> ExecuteAsync(
      Guid tenantId,
 Guid nodeId,
        CancellationToken cancellationToken = default)
    {
        var node = await _nodeRepository.GetByIdAsync(nodeId, cancellationToken);

// Validate tenant ownership
        if (node != null && node.TenantId != tenantId)
    {
       return null;
      }

        return node;
    }
}
