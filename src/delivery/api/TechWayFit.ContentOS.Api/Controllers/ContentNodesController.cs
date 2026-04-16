using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechWayFit.ContentOS.Abstractions.Security;
using TechWayFit.ContentOS.Contracts.Common;
using TechWayFit.ContentOS.Contracts.Dtos.ContentNodes;
using TechWayFit.ContentOS.Content.Application.ContentNodes;

namespace TechWayFit.ContentOS.Api.Controllers;

/// <summary>
/// Content Node management endpoints (tenant-scoped)
/// </summary>
[Authorize]
[ApiController]
[Route("api/content-nodes")]
public class ContentNodesController : ControllerBase
{
    private readonly CreateContentNodeUseCase _createNode;
 private readonly UpdateContentNodeUseCase _updateNode;
  private readonly GetContentNodeUseCase _getNode;
    private readonly GetContentNodeChildrenUseCase _getChildren;
    private readonly DeleteContentNodeUseCase _deleteNode;
    private readonly ITenantContext _tenantContext;

    public ContentNodesController(
     CreateContentNodeUseCase createNode,
  UpdateContentNodeUseCase updateNode,
    GetContentNodeUseCase getNode,
      GetContentNodeChildrenUseCase getChildren,
DeleteContentNodeUseCase deleteNode,
        ITenantContext tenantContext)
    {
        _createNode = createNode;
        _updateNode = updateNode;
        _getNode = getNode;
     _getChildren = getChildren;
_deleteNode = deleteNode;
        _tenantContext = tenantContext;
 }

  /// <summary>
    /// Create a new content node
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateNode(
      [FromBody] CreateContentNodeRequest request,
        CancellationToken cancellationToken)
    {
   var tenantId = _tenantContext.CurrentTenantId;

      var result = await _createNode.ExecuteAsync(
            tenantId,
            request.SiteId,
            request.ParentId,
          request.ContentItemId,
      request.Slug,
       request.SortOrder,
      cancellationToken);

  return result.Match<IActionResult>(
         success => Created($"/api/content-nodes/{success}",
    ApiResponse<Guid>.SuccessResponse(success, "Content node created successfully")),
   error => BadRequest(ApiResponse<object>.FailureResponse(error)));
    }

  /// <summary>
    /// Update an existing content node
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateNode(
        Guid id,
        [FromBody] UpdateContentNodeRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.CurrentTenantId;

        var result = await _updateNode.ExecuteAsync(
         tenantId,
    id,
  request.Slug,
   request.ContentItemId,
   cancellationToken);

        return result.Match<IActionResult>(
   success => NoContent(),
error => BadRequest(ApiResponse<object>.FailureResponse(error)));
    }

    /// <summary>
    /// Get a content node by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetNode(
   Guid id,
   CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.CurrentTenantId;

        var node = await _getNode.ExecuteAsync(tenantId, id, cancellationToken);

        if (node == null)
{
            return NotFound(ApiResponse<object>.FailureResponse("Content node not found"));
        }

        var response = new ContentNodeResponse
 {
       Id = node.Id,
     SiteId = node.SiteId,
  ParentId = node.ParentId,
        ContentItemId = node.ContentItemId,
   Slug = node.Slug,
      SortOrder = node.SortOrder,
            CreatedAt = node.Audit.CreatedOn,
       UpdatedAt = node.Audit.UpdatedOn
        };

return Ok(ApiResponse<ContentNodeResponse>.SuccessResponse(response));
    }

    /// <summary>
    /// Get child nodes of a parent (or root nodes if parentId not provided)
    /// </summary>
    [HttpGet("children")]
    public async Task<IActionResult> GetChildren(
        [FromQuery] Guid? parentId,
      CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.CurrentTenantId;

        var nodes = await _getChildren.ExecuteAsync(tenantId, parentId, cancellationToken);

        var response = nodes.Select(n => new ContentNodeResponse
        {
   Id = n.Id,
            SiteId = n.SiteId,
         ParentId = n.ParentId,
            ContentItemId = n.ContentItemId,
    Slug = n.Slug,
      SortOrder = n.SortOrder,
            CreatedAt = n.Audit.CreatedOn,
            UpdatedAt = n.Audit.UpdatedOn
        }).ToList();

        return Ok(ApiResponse<IReadOnlyList<ContentNodeResponse>>.SuccessResponse(response));
    }

    /// <summary>
    /// Delete a content node
    /// </summary>
    [HttpDelete("{id:guid}")]
public async Task<IActionResult> DeleteNode(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.CurrentTenantId;

        var result = await _deleteNode.ExecuteAsync(tenantId, id, cancellationToken);

        return result.Match<IActionResult>(
            success => NoContent(),
error => BadRequest(ApiResponse<object>.FailureResponse(error)));
    }
}
