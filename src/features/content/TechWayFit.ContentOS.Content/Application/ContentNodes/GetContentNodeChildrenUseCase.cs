using TechWayFit.ContentOS.Content.Domain.Hierarchy;
using TechWayFit.ContentOS.Content.Ports.Hierarchy;

namespace TechWayFit.ContentOS.Content.Application.ContentNodes;

/// <summary>
/// Use case: Get child nodes of a parent (or root nodes if parentId is null)
/// </summary>
public sealed class GetContentNodeChildrenUseCase
{
    private readonly IContentNodeRepository _nodeRepository;

    public GetContentNodeChildrenUseCase(IContentNodeRepository nodeRepository)
    {
        _nodeRepository = nodeRepository;
    }

    public async Task<IReadOnlyList<ContentNode>> ExecuteAsync(
     Guid tenantId,
     Guid? parentId,
        CancellationToken cancellationToken = default)
{
     return await _nodeRepository.GetChildrenAsync(tenantId, parentId, cancellationToken);
    }
}
