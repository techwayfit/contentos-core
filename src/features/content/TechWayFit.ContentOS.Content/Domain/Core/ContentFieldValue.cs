using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Content.Domain.Core;

public class ContentFieldValue
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ContentVersionId { get; set; }
    public string FieldKey { get; set; } = string.Empty;
    public string? Locale { get; set; }
    public string ValueJson { get; set; } = "{}";
    public AuditInfo Audit { get; set; } = new();
}
