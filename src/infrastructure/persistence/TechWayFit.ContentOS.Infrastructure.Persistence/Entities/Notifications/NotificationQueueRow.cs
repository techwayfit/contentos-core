namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Notifications;

using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;

/// <summary>
/// Notification delivery queue.
/// </summary>
public class NotificationQueueRow : BaseTenantEntity
{
    public Guid TemplateId { get; set; }
    public string RecipientEmail { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = "{}";
    public string Status { get; set; } = string.Empty;
    public DateTime ScheduledFor { get; set; }
    public DateTime? SentAt { get; set; }
    public string? ErrorMessage { get; set; }
    
    // Navigation
    public NotificationTemplateRow? Template { get; set; }
}
