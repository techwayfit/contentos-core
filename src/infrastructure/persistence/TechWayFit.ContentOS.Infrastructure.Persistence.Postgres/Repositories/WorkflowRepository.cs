using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Mappers;
using TechWayFit.ContentOS.Workflow.Domain;
using TechWayFit.ContentOS.Workflow.Ports;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Repositories;

/// <summary>
/// Repository implementation for WorkflowState using EF Core
/// </summary>
public class WorkflowRepository : IWorkflowRepository
{
    private readonly ContentOsDbContext _context;

    public WorkflowRepository(ContentOsDbContext context)
    {
        _context = context;
    }

    public async Task<WorkflowState?> GetByContentIdAsync(
        ContentItemId contentId,
        CancellationToken cancellationToken = default)
    {
        var row = await _context.WorkflowStates
            .FirstOrDefaultAsync(x => x.ContentItemId == contentId.Value, cancellationToken);

        return row == null ? null : WorkflowStateMapper.ToDomain(row);
    }

    public async Task AddAsync(WorkflowState state, CancellationToken cancellationToken = default)
    {
        var row = WorkflowStateMapper.ToRow(state);
        await _context.WorkflowStates.AddAsync(row, cancellationToken);
    }

    public Task UpdateAsync(WorkflowState state, CancellationToken cancellationToken = default)
    {
        var row = _context.WorkflowStates.FirstOrDefault(x => x.Id == state.Id.Value);

        if (row == null)
            throw new InvalidOperationException($"Workflow state {state.Id} not found for update");

        WorkflowStateMapper.UpdateRow(row, state);
        return Task.CompletedTask;
    }
}
