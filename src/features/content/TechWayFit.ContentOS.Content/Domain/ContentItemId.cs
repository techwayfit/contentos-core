namespace TechWayFit.ContentOS.Content.Domain;

/// <summary>
/// Strongly-typed content item identifier
/// </summary>
public record ContentItemId
{
    public Guid Value { get; init; }

    public ContentItemId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ContentItemId cannot be empty", nameof(value));
        Value = value;
    }

    public static ContentItemId New() => new(Guid.NewGuid());
    public static ContentItemId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
