using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Kernel.Domain.Modules;

public class ModuleMigration
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ModuleId { get; set; }
    public string MigrationKey { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
    public Guid AppliedByUserId { get; set; }
    public AuditInfo Audit { get; set; } = new();
}
