using TechWayFit.ContentOS.Content.Ports.Hierarchy;

namespace TechWayFit.ContentOS.Content.Application.Routes;

/// <summary>
/// Use case: List all routes for a content node
/// </summary>
public sealed class ListRoutesForNodeUseCase
{
    private readonly IRouteRepository _routeRepository;

 public ListRoutesForNodeUseCase(IRouteRepository routeRepository)
    {
 _routeRepository = routeRepository;
    }

public async Task<IReadOnlyList<Domain.Hierarchy.Route>> ExecuteAsync(
        Guid tenantId,
    Guid nodeId,
        CancellationToken cancellationToken = default)
    {
     var routes = await _routeRepository.GetByNodeAsync(tenantId, nodeId);
        return routes.OrderByDescending(r => r.IsPrimary).ThenBy(r => r.RoutePath).ToList();
    }
}
