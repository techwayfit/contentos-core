namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Processes;

using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;

/// <summary>
/// Domain-specific state machines and workflows.
/// </summary>
public class ProcessDefinitionRow : BaseTenantEntity
{
    public Guid ModuleId { get; set; }
    public string ProcessKey { get; set; } = string.Empty;
    public string ProcessType { get; set; } = string.Empty;
    public string StatesJson { get; set; } = "[]";
    public string TransitionsJson { get; set; } = "[]";
    public bool HasSla { get; set; }
    
    // Navigation
    public ModuleRow? Module { get; set; }
}
