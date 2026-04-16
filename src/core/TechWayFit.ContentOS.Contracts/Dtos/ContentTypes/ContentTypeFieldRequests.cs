namespace TechWayFit.ContentOS.Contracts.Dtos.ContentTypes;

/// <summary>
/// Request to add a field to a content type
/// </summary>
public record AddFieldRequest
{
    /// <summary>
    /// Unique field key (e.g., "title", "body", "publish_date")
    /// </summary>
    public required string FieldKey { get; init; }

    /// <summary>
    /// Data type: string, richtext, number, bool, datetime, ref, json
 /// </summary>
    public required string DataType { get; init; }

    /// <summary>
    /// Whether this field is required
    /// </summary>
    public bool IsRequired { get; init; }

    /// <summary>
/// Whether this field can be localized
    /// </summary>
    public bool IsLocalized { get; init; }

    /// <summary>
    /// JSON constraints (e.g., min/max length, regex pattern)
    /// </summary>
    public string? ConstraintsJson { get; init; }
}

/// <summary>
/// Request to update a content type field
/// </summary>
public record UpdateFieldRequest
{
    /// <summary>
    /// Whether this field is required
    /// </summary>
    public bool IsRequired { get; init; }

    /// <summary>
    /// Whether this field can be localized
  /// </summary>
public bool IsLocalized { get; init; }

    /// <summary>
    /// JSON constraints (e.g., min/max length, regex pattern)
    /// </summary>
    public string? ConstraintsJson { get; init; }
}

/// <summary>
/// Response containing field details
/// </summary>
public record ContentTypeFieldResponse
{
    public Guid Id { get; init; }
    public Guid ContentTypeId { get; init; }
    public string FieldKey { get; init; } = string.Empty;
    public string DataType { get; init; } = string.Empty;
    public bool IsRequired { get; init; }
    public bool IsLocalized { get; init; }
    public string ConstraintsJson { get; init; } = "{}";
    public int SortOrder { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}
