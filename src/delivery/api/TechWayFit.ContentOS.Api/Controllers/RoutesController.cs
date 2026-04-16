using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechWayFit.ContentOS.Abstractions.Security;
using TechWayFit.ContentOS.Contracts.Common;
using TechWayFit.ContentOS.Contracts.Dtos.Routes;
using TechWayFit.ContentOS.Content.Application.Routes;

namespace TechWayFit.ContentOS.Api.Controllers;

/// <summary>
/// Route management endpoints (tenant-scoped)
/// </summary>
[Authorize]
[ApiController]
[Route("api/routes")]
public class RoutesController : ControllerBase
{
    private readonly CreateRouteUseCase _createRoute;
    private readonly ResolveRouteUseCase _resolveRoute;
    private readonly ListRoutesForNodeUseCase _listRoutes;
    private readonly DeleteRouteUseCase _deleteRoute;
    private readonly ITenantContext _tenantContext;

    public RoutesController(
        CreateRouteUseCase createRoute,
        ResolveRouteUseCase resolveRoute,
        ListRoutesForNodeUseCase listRoutes,
        DeleteRouteUseCase deleteRoute,
        ITenantContext tenantContext)
    {
        _createRoute = createRoute;
   _resolveRoute = resolveRoute;
        _listRoutes = listRoutes;
 _deleteRoute = deleteRoute;
        _tenantContext = tenantContext;
    }

    /// <summary>
/// Create a new route
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateRoute(
   [FromBody] CreateRouteRequest request,
   CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.CurrentTenantId;

     var result = await _createRoute.ExecuteAsync(
    tenantId,
 request.SiteId,
  request.NodeId,
       request.RoutePath,
    request.IsPrimary,
      cancellationToken);

        return result.Match<IActionResult>(
  success => Created($"/api/routes/{success}",
    ApiResponse<Guid>.SuccessResponse(success, "Route created successfully")),
  error => BadRequest(ApiResponse<object>.FailureResponse(error)));
    }

    /// <summary>
  /// Resolve a route by path (for frontend routing)
    /// </summary>
    [HttpGet("resolve")]
    public async Task<IActionResult> ResolveRoute(
   [FromQuery] Guid siteId,
        [FromQuery] string routePath,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.CurrentTenantId;

   var route = await _resolveRoute.ExecuteAsync(tenantId, siteId, routePath, cancellationToken);

 if (route == null)
        {
     return NotFound(ApiResponse<object>.FailureResponse("Route not found"));
        }

var response = new RouteResponse
        {
   Id = route.Id,
      SiteId = route.SiteId,
   NodeId = route.NodeId,
  RoutePath = route.RoutePath,
            IsPrimary = route.IsPrimary,
     CreatedAt = route.Audit.CreatedOn
 };

        return Ok(ApiResponse<RouteResponse>.SuccessResponse(response));
    }

    /// <summary>
    /// List all routes for a node
    /// </summary>
    [HttpGet("node/{nodeId:guid}")]
 public async Task<IActionResult> ListRoutesForNode(
    Guid nodeId,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.CurrentTenantId;

        var routes = await _listRoutes.ExecuteAsync(tenantId, nodeId, cancellationToken);

        var response = routes.Select(r => new RouteResponse
        {
     Id = r.Id,
       SiteId = r.SiteId,
     NodeId = r.NodeId,
   RoutePath = r.RoutePath,
     IsPrimary = r.IsPrimary,
            CreatedAt = r.Audit.CreatedOn
        }).ToList();

     return Ok(ApiResponse<IReadOnlyList<RouteResponse>>.SuccessResponse(response));
    }

    /// <summary>
    /// Delete a route
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRoute(
        Guid id,
 CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.CurrentTenantId;

        var result = await _deleteRoute.ExecuteAsync(tenantId, id, cancellationToken);

        return result.Match<IActionResult>(
   success => NoContent(),
  error => BadRequest(ApiResponse<object>.FailureResponse(error)));
    }
}
