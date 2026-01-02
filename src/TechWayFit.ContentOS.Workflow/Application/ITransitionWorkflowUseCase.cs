using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Contracts.Dtos;

namespace TechWayFit.ContentOS.Workflow.Application;

/// <summary>
/// Command for transitioning workflow state
/// </summary>
public record TransitionWorkflowCommand(
    ContentItemId ContentId,
    string TargetState,
    string? Comment
);

/// <summary>
/// Use case for transitioning content workflow state
/// </summary>
public interface ITransitionWorkflowUseCase
{
    Task<Result<WorkflowStateResponse, Error>> ExecuteAsync(
        TransitionWorkflowCommand command,
        CancellationToken cancellationToken = default);
}
