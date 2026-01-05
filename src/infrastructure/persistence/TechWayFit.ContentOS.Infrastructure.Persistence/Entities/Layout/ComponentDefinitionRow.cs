namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Layout;

/// <summary>
/// Component registry (module-owned components + prop schema).
/// </summary>
public class ComponentDefinitionRow : BaseTenantEntity
{
    public string ComponentKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PropsSchemaJson { get; set; } = "{}";
    public string OwnerModule { get; set; } = string.Empty;
    public int Version { get; set; }
}
