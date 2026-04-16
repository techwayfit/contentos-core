namespace TechWayFit.ContentOS.Contracts.Dtos;

/// <summary>
/// Request to transition content workflow state
/// </summary>
public record WorkflowTransitionRequest
{
    public required string TargetState { get; init; }
    public string? Comment { get; init; }
}
