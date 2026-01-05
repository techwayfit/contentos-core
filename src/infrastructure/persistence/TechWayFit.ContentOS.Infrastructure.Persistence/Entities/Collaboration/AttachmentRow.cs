namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Collaboration;

using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;

/// <summary>
/// Universal file attachment support.
/// </summary>
public class AttachmentRow
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ModuleId { get; set; }
    public Guid EntityInstanceId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public string ScanStatus { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public bool IsDeleted { get; set; }
    
    // Navigation
    public ModuleRow? Module { get; set; }
}
