using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;
using TechWayFit.ContentOS.Workflow.Domain;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Mappers;

/// <summary>
/// Maps between WorkflowState domain entity and WorkflowStateRow database entity
/// </summary>
public static class WorkflowStateMapper
{
    /// <summary>
    /// Convert database row to domain entity
    /// </summary>
    public static WorkflowState ToDomain(WorkflowStateRow row)
    {
        return WorkflowState.Rehydrate(
            id: new WorkflowStateId(row.Id),
            contentItemId: new ContentItemId(row.ContentItemId),
            currentStatus: (WorkflowStatus)row.CurrentStatus,
            previousStatus: row.PreviousStatus.HasValue ? (WorkflowStatus)row.PreviousStatus.Value : null,
            transitionedBy: row.TransitionedBy.HasValue ? new UserId(row.TransitionedBy.Value) : null,
            transitionedAt: row.TransitionedAt,
            comment: row.Comment
        );
    }

    /// <summary>
    /// Convert domain entity to database row
    /// </summary>
    public static WorkflowStateRow ToRow(WorkflowState domain)
    {
        return new WorkflowStateRow
        {
            Id = domain.Id.Value,
            ContentItemId = domain.ContentItemId.Value,
            CurrentStatus = (int)domain.CurrentStatus,
            PreviousStatus = domain.PreviousStatus.HasValue ? (int)domain.PreviousStatus.Value : null,
            TransitionedBy = domain.TransitionedBy?.Value,
            TransitionedAt = domain.TransitionedAt,
            Comment = domain.Comment
        };
    }

    /// <summary>
    /// Update existing row from domain entity
    /// </summary>
    public static void UpdateRow(WorkflowStateRow row, WorkflowState domain)
    {
        row.CurrentStatus = (int)domain.CurrentStatus;
        row.PreviousStatus = domain.PreviousStatus.HasValue ? (int)domain.PreviousStatus.Value : null;
        row.TransitionedBy = domain.TransitionedBy?.Value;
        row.TransitionedAt = domain.TransitionedAt;
        row.Comment = domain.Comment;
    }
}
