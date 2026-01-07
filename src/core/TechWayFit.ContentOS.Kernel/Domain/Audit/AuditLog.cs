namespace TechWayFit.ContentOS.Kernel.Domain.Audit;

// AuditLog doesn't need full AuditInfo since it IS the audit trail
public class AuditLog
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? ActorUserId { get; set; }
    public string ActionKey { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string DetailsJson { get; set; } = "{}";
    public DateTime CreatedOn { get; set; }
}
