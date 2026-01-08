namespace TechWayFit.ContentOS.Tenancy.Domain.Identity;

/// <summary>
/// User-Group association (many-to-many)
/// </summary>
public class UserGroup
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public DateTimeOffset JoinedAt { get; set; }
}
