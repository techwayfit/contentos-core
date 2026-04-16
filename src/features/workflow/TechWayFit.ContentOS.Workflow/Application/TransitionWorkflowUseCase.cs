using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Contracts.Dtos;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Workflow.Application;

/// <summary>
/// Use case implementation for transitioning workflow state
/// </summary>
public class TransitionWorkflowUseCase : ITransitionWorkflowUseCase
{
    public Task<Result<WorkflowStateResponse, Error>> ExecuteAsync(
        TransitionWorkflowCommand command,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement workflow transition logic
        // This is a stub implementation to fix build errors
        var error = new Error
        {
            Code = "NotImplemented",
            Message = "TransitionWorkflow use case not yet implemented"
        };
        return Task.FromResult(Result.Fail<WorkflowStateResponse, Error>(error));
    }
}
