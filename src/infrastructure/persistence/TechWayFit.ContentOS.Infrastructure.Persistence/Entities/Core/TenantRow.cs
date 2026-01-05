namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;

/// <summary>
/// Top-level isolation boundary for multi-tenancy.
/// </summary>
public class TenantRow : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
