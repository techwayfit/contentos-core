using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Kernel.Domain.Modules;

public class ModuleSetting
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ModuleId { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string ValueJson { get; set; } = "{}";
    public AuditInfo Audit { get; set; } = new();
}
