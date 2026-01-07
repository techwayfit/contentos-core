using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Workflow.Domain.Core;

namespace TechWayFit.ContentOS.Workflow.Ports.Core;

/// <summary>
/// Repository interface for WorkflowState entity persistence
/// </summary>
public interface IWorkflowStateRepository : IRepository<WorkflowState, Guid>
{
    Task<IReadOnlyList<WorkflowState>> GetByWorkflowDefinitionIdAsync(Guid tenantId, Guid workflowDefinitionId, CancellationToken cancellationToken = default);
    Task<WorkflowState?> GetByStateKeyAsync(Guid tenantId, Guid workflowDefinitionId, string stateKey, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkflowState>> GetTerminalStatesAsync(Guid tenantId, Guid workflowDefinitionId, CancellationToken cancellationToken = default);
}
