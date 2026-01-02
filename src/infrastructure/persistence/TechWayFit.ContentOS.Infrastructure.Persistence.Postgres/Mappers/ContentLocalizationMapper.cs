using System.Text.Json;
using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Mappers;

/// <summary>
/// Maps between ContentLocalization domain entity and ContentLocalizationRow database entity
/// </summary>
public static class ContentLocalizationMapper
{
    /// <summary>
    /// Convert database row to domain entity
    /// </summary>
    public static ContentLocalization ToDomain(ContentLocalizationRow row)
    {
        var fields = string.IsNullOrWhiteSpace(row.FieldsJson) 
            ? new Dictionary<string, object>()
            : JsonSerializer.Deserialize<Dictionary<string, object>>(row.FieldsJson) ?? new Dictionary<string, object>();

        return ContentLocalization.Rehydrate(
            id: new ContentLocalizationId(row.Id),
            contentItemId: new ContentItemId(row.ContentItemId),
            languageCode: new LanguageCode(row.LanguageCode),
            title: new ContentTitle(row.Title),
            slug: new ContentSlug(row.Slug),
            fields: new ContentFields(fields),
            createdAt: row.CreatedAt,
            updatedAt: row.UpdatedAt
        );
    }

    /// <summary>
    /// Convert domain entity to database row
    /// </summary>
    public static ContentLocalizationRow ToRow(ContentLocalization domain)
    {
        return new ContentLocalizationRow
        {
            Id = domain.Id.Value,
            ContentItemId = domain.ContentItemId.Value,
            LanguageCode = domain.LanguageCode.Value,
            Title = domain.Title.Value,
            Slug = domain.Slug.Value,
            FieldsJson = JsonSerializer.Serialize(domain.Fields.Value),
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    /// <summary>
    /// Update existing row from domain entity
    /// </summary>
    public static void UpdateRow(ContentLocalizationRow row, ContentLocalization domain)
    {
        row.LanguageCode = domain.LanguageCode.Value;
        row.Title = domain.Title.Value;
        row.Slug = domain.Slug.Value;
        row.FieldsJson = JsonSerializer.Serialize(domain.Fields.Value);
        row.UpdatedAt = domain.UpdatedAt;
    }
}
