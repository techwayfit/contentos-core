namespace TechWayFit.ContentOS.Content.Domain;

/// <summary>
/// Value objects for Content domain
/// </summary>

public record TenantId(Guid Value);
public record SiteId(Guid Value);
public record UserId(Guid Value);

public record ContentType
{
    public string Value { get; init; }

    public ContentType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("ContentType cannot be empty", nameof(value));
        Value = value;
    }
}

public record LanguageCode
{
    public string Value { get; init; }

    public LanguageCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("LanguageCode cannot be empty", nameof(value));
        if (value.Length < 2 || value.Length > 10)
            throw new ArgumentException("LanguageCode must be between 2 and 10 characters", nameof(value));
        Value = value;
    }

    public static LanguageCode EnglishUS => new("en-US");
}

public record ContentTitle
{
    public string Value { get; init; }

    public ContentTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Title cannot be empty", nameof(value));
        if (value.Length > 500)
            throw new ArgumentException("Title cannot exceed 500 characters", nameof(value));
        Value = value;
    }
}

public record ContentSlug
{
    public string Value { get; init; }

    public ContentSlug(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Slug cannot be empty", nameof(value));
        if (value.Length > 500)
            throw new ArgumentException("Slug cannot exceed 500 characters", nameof(value));
        // Basic slug validation (alphanumeric, hyphens)
        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-z0-9-]+$"))
            throw new ArgumentException("Slug must contain only lowercase letters, numbers, and hyphens", nameof(value));
        Value = value;
    }
}

public record ContentFields
{
    public Dictionary<string, object> Value { get; init; }

    public ContentFields(Dictionary<string, object> value)
    {
        Value = value ?? new Dictionary<string, object>();
    }

    public static ContentFields Empty => new(new Dictionary<string, object>());
}
