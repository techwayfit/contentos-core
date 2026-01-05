namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Collaboration;

using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;

/// <summary>
/// Universal activity/change tracking across all modules.
/// </summary>
public class ActivityLogRow
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ModuleId { get; set; }
    public Guid EntityInstanceId { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public string ActivityKey { get; set; } = string.Empty;
    public Guid ActorUserId { get; set; }
    public string FieldChanged { get; set; } = string.Empty;
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
    public DateTime OccurredOn { get; set; }
    
    // Navigation
    public ModuleRow? Module { get; set; }
    public UserRow? Actor { get; set; }
}
