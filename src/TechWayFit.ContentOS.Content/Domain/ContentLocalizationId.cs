namespace TechWayFit.ContentOS.Content.Domain;

/// <summary>
/// Localization identifier
/// </summary>
public record ContentLocalizationId
{
    public Guid Value { get; init; }

    public ContentLocalizationId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("ContentLocalizationId cannot be empty", nameof(value));
        Value = value;
    }

    public static ContentLocalizationId New() => new(Guid.NewGuid());
    public static ContentLocalizationId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}
