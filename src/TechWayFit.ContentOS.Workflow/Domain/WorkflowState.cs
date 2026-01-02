using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Kernel.Primitives;

namespace TechWayFit.ContentOS.Workflow.Domain;

/// <summary>
/// Represents the workflow state and transition history for content
/// </summary>
public sealed class WorkflowState
{
    public WorkflowStateId Id { get; private set; }
    public ContentItemId ContentItemId { get; private set; }
    public WorkflowStatus CurrentStatus { get; private set; }
    public WorkflowStatus? PreviousStatus { get; private set; }
    public UserId? TransitionedBy { get; private set; }
    public DateTimeOffset TransitionedAt { get; private set; }
    public string? Comment { get; private set; }

    // Private constructor for EF Core
    private WorkflowState()
    {
        Id = null!;
        ContentItemId = null!;
    }

    /// <summary>
    /// Factory method to create initial workflow state
    /// </summary>
    public static WorkflowState CreateInitial(ContentItemId contentItemId, UserId createdBy)
    {
        return new WorkflowState
        {
            Id = WorkflowStateId.New(),
            ContentItemId = contentItemId,
            CurrentStatus = WorkflowStatus.Draft,
            PreviousStatus = null,
            TransitionedBy = createdBy,
            TransitionedAt = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Transition to a new workflow status
    /// </summary>
    public void Transition(WorkflowStatus newStatus, UserId userId, string? comment = null)
    {
        if (!CanTransitionTo(newStatus))
            throw new InvalidOperationException($"Cannot transition from {CurrentStatus} to {newStatus}");

        PreviousStatus = CurrentStatus;
        CurrentStatus = newStatus;
        TransitionedBy = userId;
        TransitionedAt = DateTimeOffset.UtcNow;
        Comment = comment;
    }

    /// <summary>
    /// Check if transition to target status is allowed
    /// </summary>
    public bool CanTransitionTo(WorkflowStatus targetStatus)
    {
        return CurrentStatus switch
        {
            WorkflowStatus.Draft => targetStatus is WorkflowStatus.InReview,
            WorkflowStatus.InReview => targetStatus is WorkflowStatus.Published or WorkflowStatus.Draft,
            WorkflowStatus.Published => targetStatus is WorkflowStatus.Archived or WorkflowStatus.Draft,
            WorkflowStatus.Archived => targetStatus is WorkflowStatus.Draft,
            _ => false
        };
    }

    /// <summary>
    /// Get allowed transitions from current status
    /// </summary>
    public IReadOnlyList<WorkflowStatus> GetAllowedTransitions()
    {
        return CurrentStatus switch
        {
            WorkflowStatus.Draft => new[] { WorkflowStatus.InReview },
            WorkflowStatus.InReview => new[] { WorkflowStatus.Published, WorkflowStatus.Draft },
            WorkflowStatus.Published => new[] { WorkflowStatus.Archived, WorkflowStatus.Draft },
            WorkflowStatus.Archived => new[] { WorkflowStatus.Draft },
            _ => Array.Empty<WorkflowStatus>()
        };
    }

    /// <summary>
    /// Rehydrate from persistence (for mappers)
    /// </summary>
    public static WorkflowState Rehydrate(
        WorkflowStateId id,
        ContentItemId contentItemId,
        WorkflowStatus currentStatus,
        WorkflowStatus? previousStatus,
        UserId? transitionedBy,
        DateTimeOffset transitionedAt,
        string? comment)
    {
        return new WorkflowState
        {
            Id = id,
            ContentItemId = contentItemId,
            CurrentStatus = currentStatus,
            PreviousStatus = previousStatus,
            TransitionedBy = transitionedBy,
            TransitionedAt = transitionedAt,
            Comment = comment
        };
    }
}
