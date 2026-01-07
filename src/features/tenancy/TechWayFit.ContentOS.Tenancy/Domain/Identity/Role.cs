using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Tenancy.Domain.Identity;

/// <summary>
/// Role domain entity - Pure POCO
/// Role-based access control (RBAC)
/// </summary>
public sealed class Role
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = default!;
    public AuditInfo Audit { get; set; } = new();
}
