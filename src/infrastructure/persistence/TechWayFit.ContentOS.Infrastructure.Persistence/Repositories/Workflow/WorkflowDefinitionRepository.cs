using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Workflow;
using TechWayFit.ContentOS.Workflow.Domain.Core;
using TechWayFit.ContentOS.Workflow.Ports.Core;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Workflow;

public class WorkflowDefinitionRepository : EfCoreRepository<WorkflowDefinition, WorkflowDefinitionRow, Guid>, IWorkflowDefinitionRepository
{
    public WorkflowDefinitionRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override WorkflowDefinition MapToDomain(WorkflowDefinitionRow row)
    {
        return new WorkflowDefinition
        {
            Id = row.Id,
            TenantId = row.TenantId,
            WorkflowKey = row.WorkflowKey,
            DisplayName = row.DisplayName,
            IsDefault = row.IsDefault,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override WorkflowDefinitionRow MapToRow(WorkflowDefinition entity)
    {
        var row = new WorkflowDefinitionRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            WorkflowKey = entity.WorkflowKey,
            DisplayName = entity.DisplayName,
            IsDefault = entity.IsDefault
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<WorkflowDefinitionRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<WorkflowDefinition?> GetByKeyAsync(Guid tenantId, string workflowKey)
    {
        var row = await DbSet
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.WorkflowKey == workflowKey);
        return row != null ? MapToDomain(row) : null;
    }

    public async Task<WorkflowDefinition?> GetDefaultAsync(Guid tenantId)
    {
        var row = await DbSet
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.IsDefault);
        return row != null ? MapToDomain(row) : null;
    }

    public async Task<IEnumerable<WorkflowDefinition>> GetByTenantAsync(Guid tenantId)
    {
        var rows = await DbSet
            .Where(r => r.TenantId == tenantId)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }
}
