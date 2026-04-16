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

    public async Task<WorkflowDefinition?> GetByWorkflowKeyAsync(Guid tenantId, string workflowKey, CancellationToken cancellationToken = default)
    {
        var row = await Context.Set<WorkflowDefinitionRow>()
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.WorkflowKey == workflowKey, cancellationToken);
        return row != null ? MapToDomain(row) : null;
    }

    public async Task<WorkflowDefinition?> GetDefaultAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var row = await Context.Set<WorkflowDefinitionRow>()
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.IsDefault, cancellationToken);
        return row != null ? MapToDomain(row) : null;
    }

    public async Task<IReadOnlyList<WorkflowDefinition>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var rows = await Context.Set<WorkflowDefinitionRow>()
            .Where(r => r.TenantId == tenantId)
            .ToListAsync(cancellationToken);
        return rows.Select(MapToDomain).ToList();
    }

    // Legacy methods (keeping for backward compatibility)
    public async Task<WorkflowDefinition?> GetByKeyAsync(Guid tenantId, string workflowKey)
    {
        var row = await Context.Set<WorkflowDefinitionRow>()
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.WorkflowKey == workflowKey);
        return row != null ? MapToDomain(row) : null;
    }

    public async Task<IEnumerable<WorkflowDefinition>> GetByTenantAsync(Guid tenantId)
    {
        var rows = await Context.Set<WorkflowDefinitionRow>()
            .Where(r => r.TenantId == tenantId)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }
}
