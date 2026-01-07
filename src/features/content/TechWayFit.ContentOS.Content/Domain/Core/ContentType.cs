using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Content.Domain.Core;

/// <summary>
/// ContentType domain entity - Pure POCO
/// Schema registry for content modeling (Contentful-like)
/// </summary>
public sealed class ContentType
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string TypeKey { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public int SchemaVersion { get; set; }
    public string SettingsJson { get; set; } = "{}";
    public AuditInfo Audit { get; set; } = new();
}
