namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Layout;

/// <summary>
/// Reusable layout template rules (regions + allowed components).
/// </summary>
public class LayoutDefinitionRow : BaseTenantEntity
{
    public string LayoutKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string RegionsRulesJson { get; set; } = "{}";
    public int Version { get; set; }
}
