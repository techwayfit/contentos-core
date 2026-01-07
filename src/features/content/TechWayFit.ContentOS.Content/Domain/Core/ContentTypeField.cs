using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Content.Domain.Core;

public class ContentTypeField
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ContentTypeId { get; set; }
    public string FieldKey { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty; // string|richtext|number|bool|datetime|ref|json
    public bool IsRequired { get; set; }
    public bool IsLocalized { get; set; }
    public string ConstraintsJson { get; set; } = "{}";
    public int SortOrder { get; set; }
    public AuditInfo Audit { get; set; } = new();
}
