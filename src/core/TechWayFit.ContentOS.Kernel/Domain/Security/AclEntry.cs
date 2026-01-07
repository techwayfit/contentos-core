using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Kernel.Domain.Security;

public class AclEntry
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string ScopeType { get; set; } = string.Empty; // Tenant|Site|Node|ContentType
    public Guid ScopeId { get; set; }
    public string PrincipalType { get; set; } = string.Empty; // User|Role|Group
    public Guid PrincipalId { get; set; }
    public string Effect { get; set; } = string.Empty; // Allow|Deny
    public string ActionsCsv { get; set; } = string.Empty;
    public AuditInfo Audit { get; set; } = new();
}
