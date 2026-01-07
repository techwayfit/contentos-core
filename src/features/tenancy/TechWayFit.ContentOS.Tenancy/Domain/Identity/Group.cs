using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Tenancy.Domain.Identity;

/// <summary>
/// Group domain entity - Pure POCO
/// User groups for organizational structure
/// </summary>
public sealed class Group
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = default!;
    public AuditInfo Audit { get; set; } = new();
}
