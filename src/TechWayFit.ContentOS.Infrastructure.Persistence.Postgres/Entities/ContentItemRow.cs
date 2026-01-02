using TechWayFit.ContentOS.Infrastructure.Persistence.Contracts;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;

/// <summary>
/// Database row model for content items (NOT domain entity)
/// Represents the language-agnostic content shell
/// </summary>
public sealed class ContentItemRow : ITenantOwnedRow
{
    public Guid Id { get; set; }

    // Multi-tenant fields (ALWAYS required for tenant isolation)
    public Guid TenantId { get; set; }
    public Guid SiteId { get; set; }
    public string Environment { get; set; } = default!;

    // Content metadata
    public string ContentType { get; set; } = default!;
    public string DefaultLanguage { get; set; } = default!;
    public int WorkflowStatus { get; set; }

    // Audit fields
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }

    // Navigation properties
    public List<ContentLocalizationRow> Localizations { get; set; } = new();
}
