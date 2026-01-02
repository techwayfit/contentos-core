namespace TechWayFit.ContentOS.Workflow.Domain;

/// <summary>
/// Workflow state identifier
/// </summary>
public record WorkflowStateId
{
    public Guid Value { get; init; }

    public WorkflowStateId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("WorkflowStateId cannot be empty", nameof(value));
        Value = value;
    }

    public static WorkflowStateId New() => new(Guid.NewGuid());
    public static WorkflowStateId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
