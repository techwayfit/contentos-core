using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Workflow;
using TechWayFit.ContentOS.Workflow.Domain.Core;
using TechWayFit.ContentOS.Workflow.Ports.Core;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Workflow;

public class WorkflowTransitionRepository : EfCoreRepository<WorkflowTransition, WorkflowTransitionRow, Guid>, IWorkflowTransitionRepository
{
    public WorkflowTransitionRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override WorkflowTransition MapToDomain(WorkflowTransitionRow row)
    {
        return new WorkflowTransition
        {
            Id = row.Id,
            TenantId = row.TenantId,
            WorkflowDefinitionId = row.WorkflowDefinitionId,
            FromStateId = row.FromStateId,
            ToStateId = row.ToStateId,
            RequiredAction = row.RequiredAction,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override WorkflowTransitionRow MapToRow(WorkflowTransition entity)
    {
        var row = new WorkflowTransitionRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            WorkflowDefinitionId = entity.WorkflowDefinitionId,
            FromStateId = entity.FromStateId,
            ToStateId = entity.ToStateId,
            RequiredAction = entity.RequiredAction
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<WorkflowTransitionRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<IEnumerable<WorkflowTransition>> GetByDefinitionAsync(Guid tenantId, Guid workflowDefinitionId)
    {
        var rows = await DbSet
            .Where(r => r.TenantId == tenantId && r.WorkflowDefinitionId == workflowDefinitionId)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task<IEnumerable<WorkflowTransition>> GetAllowedTransitionsAsync(Guid tenantId, Guid fromStateId)
    {
        var rows = await DbSet
            .Where(r => r.TenantId == tenantId && r.FromStateId == fromStateId)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }
}
