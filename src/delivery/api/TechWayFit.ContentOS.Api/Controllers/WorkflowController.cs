using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Contracts.Dtos;
using TechWayFit.ContentOS.Infrastructure.Runtime;
using TechWayFit.ContentOS.Api.Tenancy;
using TechWayFit.ContentOS.Workflow.Application;

namespace TechWayFit.ContentOS.Api.Controllers;

/// <summary>
/// Content workflow and publishing operations
/// </summary>
[ApiController]
[Route("api/content/{contentId}/workflow")]
[Produces("application/json")]
[SwaggerTag("Manage content workflow states and transitions (Draft ? Review ? Published)")]
public class WorkflowController : ControllerBase
{
    private readonly ITransitionWorkflowUseCase _transitionWorkflow;
    private readonly TenantContext _tenantContext;
    private readonly ILogger<WorkflowController> _logger;

    public WorkflowController(
     ITransitionWorkflowUseCase transitionWorkflow,
        TenantContext tenantContext,
     ILogger<WorkflowController> logger)
    {
        _transitionWorkflow = transitionWorkflow;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    /// <summary>
    /// Transition content workflow state
    /// </summary>
    /// <param name="contentId">Content item ID</param>
    /// <param name="request">Transition details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated workflow state</returns>
 /// <remarks>
    /// Transitions content through workflow states based on defined workflow rules.
    /// 
    /// **Common Workflow Transitions:**
 /// - Draft ? InReview (submit for approval)
    /// - InReview ? Approved (approve content)
    /// - InReview ? Draft (reject/revise)
  /// - Approved ? Published (publish live)
    /// - Published ? Archived (archive/unpublish)
    /// 
    /// Sample request:
    /// 
    ///     POST /api/content/550e8400-e29b-41d4-a716-446655440000/workflow/transition
    ///     {
    ///       "targetState": "Published",
    ///       "comment": "Ready for production deployment"
    ///     }
    /// 
    /// **Permissions Required:**
    /// - `workflow:transition` (minimum)
    /// - Specific state transitions may require additional permissions
    /// 
    /// **Validation Rules:**
    /// - User must have permission for the target state
    /// - Transition must be allowed by workflow definition
 /// - Content must meet all validation requirements
    /// </remarks>
    /// <response code="200">Workflow transitioned successfully</response>
    /// <response code="400">Invalid transition or validation failed</response>
    /// <response code="404">Content or workflow not found</response>
    /// <response code="403">Insufficient permissions for transition</response>
    [HttpPost("transition")]
    [SwaggerOperation(
        Summary = "Transition workflow state",
     Description = "Moves content through workflow states (e.g., Draft ? Published)",
        OperationId = "TransitionWorkflow",
        Tags = new[] { "Workflow", "Publishing" }
    )]
    [SwaggerResponse(200, "Workflow transitioned", typeof(WorkflowStateResponse))]
    [SwaggerResponse(400, "Invalid transition", typeof(Error))]
    [SwaggerResponse(404, "Content or workflow not found", typeof(Error))]
    [SwaggerResponse(403, "Forbidden - insufficient permissions")]
    [ProducesResponseType(typeof(WorkflowStateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<WorkflowStateResponse>> TransitionWorkflow(
        [FromRoute, SwaggerParameter("Content item identifier", Required = true)] Guid contentId,
        [FromBody, SwaggerRequestBody("Workflow transition request", Required = true)] WorkflowTransitionRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
        "Transitioning workflow for content {ContentId} to {TargetState}",
 contentId,
        request.TargetState);

        var command = new TransitionWorkflowCommand(
       new ContentItemId(contentId),
 request.TargetState,
        request.Comment);

        var result = await _transitionWorkflow.ExecuteAsync(command, cancellationToken);

        return result.Match<ActionResult<WorkflowStateResponse>>(
            success => Ok(success),
            error => error.Code switch
     {
      "ContentNotFound" => NotFound(error),
  "WorkflowNotFound" => NotFound(error),
           "InvalidTransition" => BadRequest(error),
        _ => BadRequest(error)
          });
    }

    /// <summary>
    /// Get current workflow state
    /// </summary>
    /// <param name="contentId">Content item ID</param>
    /// <returns>Current workflow state and available transitions</returns>
    /// <remarks>
    /// Retrieves the current workflow state for a content item,
    /// including available transitions based on user permissions.
    /// 
    /// **Response includes:**
    /// - Current state name and metadata
    /// - Available transitions for current user
    /// - Workflow history (optional)
  /// - Required permissions for each transition
    /// 
    /// **Note:** This endpoint is not yet implemented.
    /// </remarks>
    /// <response code="200">Workflow state retrieved</response>
    /// <response code="404">Content not found</response>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get workflow state",
        Description = "Retrieves current workflow state and available transitions (not yet implemented)",
        OperationId = "GetWorkflowState",
      Tags = new[] { "Workflow" }
    )]
    [SwaggerResponse(200, "Workflow state found", typeof(WorkflowStateResponse))]
    [SwaggerResponse(404, "Content not found")]
    [ProducesResponseType(typeof(WorkflowStateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<WorkflowStateResponse> GetWorkflowState(
[FromRoute, SwaggerParameter("Content item identifier", Required = true)] Guid contentId)
    {
     // TODO: Implement GetWorkflowStateUseCase
      _logger.LogWarning("GetWorkflowState not yet implemented for {ContentId}", contentId);
        return NotFound();
    }
}
