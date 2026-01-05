namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities;

/// <summary>
/// Base entity with Id and full audit fields.
/// All database entities should inherit from this or its derived classes.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    
    // Audit fields
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
