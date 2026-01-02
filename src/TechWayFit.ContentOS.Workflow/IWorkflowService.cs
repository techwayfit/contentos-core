namespace TechWayFit.ContentOS.Workflow;

/// <summary>
/// Manages content workflow states (draft, review, publish)
/// </summary>
public interface IWorkflowService
{
    Task<object> TransitionAsync(string contentId, string targetState, CancellationToken cancellationToken = default);
    Task<object> GetWorkflowStatusAsync(string contentId, CancellationToken cancellationToken = default);
}
