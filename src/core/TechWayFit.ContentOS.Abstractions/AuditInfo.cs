namespace TechWayFit.ContentOS.Abstractions;

/// <summary>
/// Audit information for domain entities.
/// Groups all audit-related fields together using composition.
/// </summary>
public sealed class AuditInfo
{
    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime UpdatedOn { get; set; }
    public Guid UpdatedBy { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
    public bool CanDelete { get; set; }
    public bool IsSystem { get; set; }
}
