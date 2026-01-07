using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Workflow;
using TechWayFit.ContentOS.Workflow.Domain.Core;
using TechWayFit.ContentOS.Workflow.Ports.Core;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Workflow;

public class WorkflowStateRepository : EfCoreRepository<WorkflowState, WorkflowStateRow, Guid>, IWorkflowStateRepository
{
    public WorkflowStateRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override WorkflowState MapToDomain(WorkflowStateRow row)
    {
        return new WorkflowState
        {
            Id = row.Id,
            TenantId = row.TenantId,
            WorkflowDefinitionId = row.WorkflowDefinitionId,
            StateKey = row.StateKey,
            DisplayName = row.DisplayName,
            IsTerminal = row.IsTerminal,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override WorkflowStateRow MapToRow(WorkflowState entity)
    {
        var row = new WorkflowStateRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            WorkflowDefinitionId = entity.WorkflowDefinitionId,
            StateKey = entity.StateKey,
            DisplayName = entity.DisplayName,
            IsTerminal = entity.IsTerminal
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<WorkflowStateRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<IEnumerable<WorkflowState>> GetByDefinitionAsync(Guid tenantId, Guid workflowDefinitionId)
    {
        var rows = await Context.Set<WorkflowStateRow>()
            .Where(r => r.TenantId == tenantId && r.WorkflowDefinitionId == workflowDefinitionId)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task<WorkflowState?> GetByKeyAsync(Guid tenantId, Guid workflowDefinitionId, string stateKey)
    {
        var row = await Context.Set<WorkflowStateRow>()
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.WorkflowDefinitionId == workflowDefinitionId && r.StateKey == stateKey);
        return row != null ? MapToDomain(row) : null;
    }

    public Task<IReadOnlyList<WorkflowState>> GetByWorkflowDefinitionIdAsync(Guid tenantId, Guid workflowDefinitionId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<WorkflowState?> GetByStateKeyAsync(Guid tenantId, Guid workflowDefinitionId, string stateKey, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<WorkflowState>> GetTerminalStatesAsync(Guid tenantId, Guid workflowDefinitionId, CancellationToken cancellationToken = default)
    {
           var rows = await Context.Set<WorkflowStateRow>()
            .Where(r => r.TenantId == tenantId && r.WorkflowDefinitionId == workflowDefinitionId && r.IsTerminal)
            .ToListAsync();
        return rows.Select(MapToDomain).ToList();
    }
}
