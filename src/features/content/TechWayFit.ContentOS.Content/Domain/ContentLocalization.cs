namespace TechWayFit.ContentOS.Content.Domain;

/// <summary>
/// Represents a localized version of content
/// Contains language-specific title, slug, and fields
/// </summary>
public sealed class ContentLocalization
{
    public ContentLocalizationId Id { get; private set; }
    public ContentItemId ContentItemId { get; private set; }
    public LanguageCode LanguageCode { get; private set; }
    public ContentTitle Title { get; private set; }
    public ContentSlug Slug { get; private set; }
    public ContentFields Fields { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    // Private constructor for EF Core
    private ContentLocalization()
    {
        Id = null!;
        ContentItemId = null!;
        LanguageCode = null!;
        Title = null!;
        Slug = null!;
        Fields = null!;
    }

    /// <summary>
    /// Factory method to create a new localization
    /// </summary>
    public static ContentLocalization Create(
        ContentItemId contentItemId,
        LanguageCode languageCode,
        ContentTitle title,
        ContentSlug slug,
        ContentFields fields)
    {
        return new ContentLocalization
        {
            Id = ContentLocalizationId.New(),
            ContentItemId = contentItemId,
            LanguageCode = languageCode,
            Title = title,
            Slug = slug,
            Fields = fields,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Update the localization content
    /// </summary>
    public void Update(ContentTitle title, ContentSlug slug, ContentFields fields)
    {
        Title = title;
        Slug = slug;
        Fields = fields;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Rehydrate from persistence (for mappers)
    /// </summary>
    public static ContentLocalization Rehydrate(
        ContentLocalizationId id,
        ContentItemId contentItemId,
        LanguageCode languageCode,
        ContentTitle title,
        ContentSlug slug,
        ContentFields fields,
        DateTimeOffset createdAt,
        DateTimeOffset? updatedAt)
    {
        return new ContentLocalization
        {
            Id = id,
            ContentItemId = contentItemId,
            LanguageCode = languageCode,
            Title = title,
            Slug = slug,
            Fields = fields,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }
}
