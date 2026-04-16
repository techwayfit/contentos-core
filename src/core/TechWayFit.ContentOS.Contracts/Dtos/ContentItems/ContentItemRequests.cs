namespace TechWayFit.ContentOS.Contracts.Dtos.ContentItems;

/// <summary>
/// Request to create a content item
/// </summary>
public record CreateContentItemRequest
{
    /// <summary>
    /// Site ID
    /// </summary>
    public required Guid SiteId { get; init; }

/// <summary>
  /// Content type ID
    /// </summary>
    public required Guid ContentTypeId { get; init; }
}

/// <summary>
/// Request to update a content item
/// </summary>
public record UpdateContentItemRequest
{
  /// <summary>
    /// Status (draft, review, published, archived)
    /// </summary>
    public required string Status { get; init; }
}

/// <summary>
/// Response containing content item details
/// </summary>
public record ContentItemResponse
{
    public Guid Id { get; init; }
    public Guid SiteId { get; init; }
    public Guid ContentTypeId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}

/// <summary>
/// Request to update field values
/// </summary>
public record UpdateFieldValuesRequest
{
 /// <summary>
  /// Field key-value pairs (value as JSON string)
    /// </summary>
    public required Dictionary<string, string> FieldValues { get; init; }

  /// <summary>
    /// Locale for localized fields (optional)
    /// </summary>
    public string? Locale { get; init; }
}

/// <summary>
/// Response containing field value details
/// </summary>
public record ContentFieldValueResponse
{
    public Guid Id { get; init; }
    public string FieldKey { get; init; } = string.Empty;
    public string? Locale { get; init; }
    public string ValueJson { get; init; } = "{}";
}
