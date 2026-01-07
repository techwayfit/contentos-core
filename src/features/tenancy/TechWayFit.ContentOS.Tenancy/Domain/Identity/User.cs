using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Tenancy.Domain.Identity;

/// <summary>
/// User domain entity - Pure POCO
/// Core identity record (auth handled externally via IdP)
/// </summary>
public sealed class User
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Email { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Status { get; set; } = default!;
    public AuditInfo Audit { get; set; } = new();
}
