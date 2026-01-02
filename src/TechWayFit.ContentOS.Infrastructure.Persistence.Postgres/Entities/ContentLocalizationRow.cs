namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;

/// <summary>
/// Database row model for content localizations (NOT domain entity)
/// Contains language-specific content data
/// </summary>
public sealed class ContentLocalizationRow
{
    public Guid Id { get; set; }
    public Guid ContentItemId { get; set; }
    public string LanguageCode { get; set; } = default!; // "en-US", "fr-FR"

    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string FieldsJson { get; set; } = "{}"; // JSON blob for flexible fields

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    // Navigation property
    public ContentItemRow ContentItem { get; set; } = default!;
}
