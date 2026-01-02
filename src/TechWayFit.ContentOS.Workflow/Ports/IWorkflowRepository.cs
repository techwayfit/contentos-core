using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Workflow.Domain;

namespace TechWayFit.ContentOS.Workflow.Ports;

/// <summary>
/// Repository port for workflow state persistence
/// Implementation in Infrastructure layer
/// </summary>
public interface IWorkflowRepository
{
    Task<WorkflowState?> GetByContentIdAsync(ContentItemId contentId, CancellationToken cancellationToken = default);
    Task AddAsync(WorkflowState state, CancellationToken cancellationToken = default);
    Task UpdateAsync(WorkflowState state, CancellationToken cancellationToken = default);
}
