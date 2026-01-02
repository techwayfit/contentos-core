namespace TechWayFit.ContentOS.Contracts.Dtos;

/// <summary>
/// Response containing workflow state information
/// </summary>
public record WorkflowStateResponse
{
    public required Guid ContentId { get; init; }
    public required string CurrentStatus { get; init; }
    public string? PreviousStatus { get; init; }
    public Guid? TransitionedBy { get; init; }
    public DateTimeOffset? TransitionedAt { get; init; }
    public string? Comment { get; init; }
    public List<string> AllowedTransitions { get; init; } = new();
}
