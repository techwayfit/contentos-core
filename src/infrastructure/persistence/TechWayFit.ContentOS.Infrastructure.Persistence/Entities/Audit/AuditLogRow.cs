namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Audit;

/// <summary>
/// Records important actions (publish, permission changes, schema changes).
/// Append-only table - no updates or deletes allowed.
/// </summary>
public class AuditLogRow : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid? ActorUserId { get; set; }
    public string ActionKey { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string DetailsJson { get; set; } = "{}";
    // CreatedOn is inherited from BaseEntity
}
