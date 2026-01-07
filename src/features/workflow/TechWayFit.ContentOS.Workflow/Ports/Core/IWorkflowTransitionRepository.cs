using TechWayFit.ContentOS.Abstractions.Repositories;

namespace TechWayFit.ContentOS.Workflow.Ports.Core;

public interface IWorkflowTransitionRepository : IRepository<Domain.Core.WorkflowTransition, Guid>
{
    Task<IEnumerable<Domain.Core.WorkflowTransition>> GetByDefinitionAsync(Guid tenantId, Guid workflowDefinitionId);
    Task<IEnumerable<Domain.Core.WorkflowTransition>> GetAllowedTransitionsAsync(Guid tenantId, Guid fromStateId);
}
