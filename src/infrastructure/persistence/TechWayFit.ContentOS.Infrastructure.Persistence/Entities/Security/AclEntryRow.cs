namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Security;

/// <summary>
/// Fine-grained permissions on scopes with inheritance.
/// </summary>
public class AclEntryRow : BaseTenantEntity
{
    public string ScopeType { get; set; } = string.Empty; // Tenant|Site|Node|ContentType
    public Guid ScopeId { get; set; }
    public string PrincipalType { get; set; } = string.Empty; // User|Role|Group
    public Guid PrincipalId { get; set; }
    public string Effect { get; set; } = string.Empty; // Allow|Deny
    public string ActionsCsv { get; set; } = string.Empty;
}
