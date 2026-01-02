using System.Text.Json;
using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;
using TechWayFit.ContentOS.Kernel.Primitives;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Mappers;

/// <summary>
/// Maps between ContentItem domain entity and ContentItemRow database entity
/// This mapping lives in Infrastructure to keep domain clean
/// </summary>
public static class ContentItemMapper
{
    /// <summary>
    /// Convert database row to domain entity
    /// </summary>
    public static ContentItem ToDomain(ContentItemRow row)
    {
        var localizations = row.Localizations
            .Select(ContentLocalizationMapper.ToDomain)
            .ToList();

        return ContentItem.Rehydrate(
            id: new ContentItemId(row.Id),
            tenantId: new TenantId(row.TenantId),
            siteId: new SiteId(row.SiteId),
            contentType: new ContentType(row.ContentType),
            defaultLanguage: new LanguageCode(row.DefaultLanguage),
            status: (WorkflowStatus)row.WorkflowStatus,
            localizations: localizations,
            createdAt: row.CreatedAt,
            updatedAt: row.UpdatedAt,
            createdBy: row.CreatedBy.HasValue ? new UserId(row.CreatedBy.Value) : null,
            updatedBy: row.UpdatedBy.HasValue ? new UserId(row.UpdatedBy.Value) : null
        );
    }

    /// <summary>
    /// Convert domain entity to database row
    /// </summary>
    public static ContentItemRow ToRow(ContentItem domain)
    {
        return new ContentItemRow
        {
            Id = domain.Id.Value,
            TenantId = domain.TenantId.Value,
            SiteId = domain.SiteId.Value,
            Environment = "production", // Default for now, could come from tenant context
            ContentType = domain.ContentType.Value,
            DefaultLanguage = domain.DefaultLanguage.Value,
            WorkflowStatus = (int)domain.Status,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt,
            CreatedBy = domain.CreatedBy?.Value,
            UpdatedBy = domain.UpdatedBy?.Value,
            Localizations = domain.Localizations
                .Select(ContentLocalizationMapper.ToRow)
                .ToList()
        };
    }

    /// <summary>
    /// Update existing row from domain entity (for updates)
    /// </summary>
    public static void UpdateRow(ContentItemRow row, ContentItem domain)
    {
        row.ContentType = domain.ContentType.Value;
        row.DefaultLanguage = domain.DefaultLanguage.Value;
        row.WorkflowStatus = (int)domain.Status;
        row.UpdatedAt = domain.UpdatedAt;
        row.UpdatedBy = domain.UpdatedBy?.Value;

        // Update or add localizations
        foreach (var domainLoc in domain.Localizations)
        {
            var existingRow = row.Localizations.FirstOrDefault(l => l.Id == domainLoc.Id.Value);
            if (existingRow != null)
            {
                ContentLocalizationMapper.UpdateRow(existingRow, domainLoc);
            }
            else
            {
                row.Localizations.Add(ContentLocalizationMapper.ToRow(domainLoc));
            }
        }
    }
}
