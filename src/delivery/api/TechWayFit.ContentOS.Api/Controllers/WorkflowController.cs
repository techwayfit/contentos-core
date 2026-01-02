using Microsoft.AspNetCore.Mvc;
using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Contracts.Dtos;
using TechWayFit.ContentOS.Infrastructure.Runtime;
using TechWayFit.ContentOS.Api.Tenancy;
using TechWayFit.ContentOS.Workflow.Application;

namespace TechWayFit.ContentOS.Api.Controllers;

[ApiController]
[Route("api/content/{contentId}/workflow")]
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

    [HttpPost("transition")]
    public async Task<ActionResult<WorkflowStateResponse>> TransitionWorkflow(
        Guid contentId,
        [FromBody] WorkflowTransitionRequest request,
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

    [HttpGet]
    public ActionResult<WorkflowStateResponse> GetWorkflowState(Guid contentId)
    {
        // TODO: Implement GetWorkflowStateUseCase
        _logger.LogWarning("GetWorkflowState not yet implemented for {ContentId}", contentId);
        return NotFound();
    }
}
