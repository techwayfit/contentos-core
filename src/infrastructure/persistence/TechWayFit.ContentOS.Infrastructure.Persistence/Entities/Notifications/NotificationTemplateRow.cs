namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Notifications;

using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;

/// <summary>
/// Multi-channel notification templates.
/// </summary>
public class NotificationTemplateRow : BaseTenantEntity
{
    public string TemplateKey { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    
    // Navigation
    public ModuleRow? Module { get; set; }
}
