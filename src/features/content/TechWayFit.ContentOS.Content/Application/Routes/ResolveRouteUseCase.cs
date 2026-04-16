using TechWayFit.ContentOS.Content.Ports.Hierarchy;

namespace TechWayFit.ContentOS.Content.Application.Routes;

/// <summary>
/// Use case: Resolve a route by path (for frontend routing)
/// </summary>
public sealed class ResolveRouteUseCase
{
    private readonly IRouteRepository _routeRepository;

    public ResolveRouteUseCase(IRouteRepository routeRepository)
    {
        _routeRepository = routeRepository;
    }

    public async Task<Domain.Hierarchy.Route?> ExecuteAsync(
        Guid tenantId,
        Guid siteId,
        string routePath,
        CancellationToken cancellationToken = default)
    {
        return await _routeRepository.GetByRoutePathAsync(tenantId, siteId, routePath);
    }
}
