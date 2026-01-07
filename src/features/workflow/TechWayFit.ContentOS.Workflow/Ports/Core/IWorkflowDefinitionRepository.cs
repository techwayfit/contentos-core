using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Workflow.Domain.Core;

namespace TechWayFit.ContentOS.Workflow.Ports.Core;

/// <summary>
/// Repository interface for WorkflowDefinition entity persistence
/// </summary>
public interface IWorkflowDefinitionRepository : IRepository<WorkflowDefinition, Guid>
{
    Task<WorkflowDefinition?> GetByWorkflowKeyAsync(Guid tenantId, string workflowKey, CancellationToken cancellationToken = default);
    Task<WorkflowDefinition?> GetDefaultAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<WorkflowDefinition>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
