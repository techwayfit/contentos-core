using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Ports.Hierarchy;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Content.Application.Routes;

/// <summary>
/// Use case: Create a route for a content node
/// </summary>
public sealed class CreateRouteUseCase
{
  private readonly IRouteRepository _routeRepository;
    private readonly IContentNodeRepository _nodeRepository;
 private readonly IUnitOfWork _unitOfWork;

    public CreateRouteUseCase(
        IRouteRepository routeRepository,
        IContentNodeRepository nodeRepository,
      IUnitOfWork unitOfWork)
 {
        _routeRepository = routeRepository;
        _nodeRepository = nodeRepository;
   _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, string>> ExecuteAsync(
      Guid tenantId,
        Guid siteId,
        Guid nodeId,
        string routePath,
        bool isPrimary,
   CancellationToken cancellationToken = default)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(routePath))
            return Result.Fail<Guid, string>("Route path cannot be empty");

      // Validate route path format (should start with /)
  if (!routePath.StartsWith("/"))
       return Result.Fail<Guid, string>("Route path must start with '/'");

        // Validate node exists
   var node = await _nodeRepository.GetByIdAsync(nodeId, cancellationToken);
        if (node == null || node.TenantId != tenantId || node.SiteId != siteId)
        {
          return Result.Fail<Guid, string>($"Node with ID '{nodeId}' not found");
   }

        // Check if route path already exists
        var existingRoute = await _routeRepository.GetByRoutePathAsync(tenantId, siteId, routePath);
      if (existingRoute != null)
     {
     return Result.Fail<Guid, string>($"Route path '{routePath}' already exists");
        }

        // If setting as primary, clear existing primary route
        if (isPrimary)
        {
     var existingPrimary = await _routeRepository.GetPrimaryAsync(tenantId, nodeId);
            if (existingPrimary != null)
       {
       existingPrimary.IsPrimary = false;
   await _routeRepository.UpdateAsync(existingPrimary, cancellationToken);
     }
        }

        // Create route
        var route = new Domain.Hierarchy.Route
   {
   Id = Guid.NewGuid(),
        TenantId = tenantId,
  SiteId = siteId,
        NodeId = nodeId,
          RoutePath = routePath,
            IsPrimary = isPrimary,
     Audit = new AuditInfo
    {
       CreatedOn = DateTime.UtcNow
         }
        };

        // Persist
        await _routeRepository.AddAsync(route, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish RouteCreated event

        return Result.Ok<Guid, string>(route.Id);
    }
}
