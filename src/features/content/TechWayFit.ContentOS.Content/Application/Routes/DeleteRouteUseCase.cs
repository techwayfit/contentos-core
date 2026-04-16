using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Ports.Hierarchy;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application.Routes;

/// <summary>
/// Use case: Delete a route
/// </summary>
public sealed class DeleteRouteUseCase
{
  private readonly IRouteRepository _routeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRouteUseCase(
        IRouteRepository routeRepository,
        IUnitOfWork unitOfWork)
{
        _routeRepository = routeRepository;
     _unitOfWork = unitOfWork;
  }

    public async Task<Result<bool, string>> ExecuteAsync(
      Guid tenantId,
   Guid routeId,
        CancellationToken cancellationToken = default)
    {
        // Get existing route
        var route = await _routeRepository.GetByIdAsync(routeId, cancellationToken);
  if (route == null || route.TenantId != tenantId)
 {
      return Result.Fail<bool, string>($"Route with ID '{routeId}' not found");
        }

        // Prevent deletion of primary route if it's the only route
    if (route.IsPrimary)
        {
     var allRoutes = await _routeRepository.GetByNodeAsync(tenantId, route.NodeId);
            if (allRoutes.Count() == 1)
  {
                return Result.Fail<bool, string>("Cannot delete the only route for this node");
   }
     }

  // Delete route
   await _routeRepository.DeleteAsync(routeId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish RouteDeleted event

  return Result.Ok<bool, string>(true);
    }
}
