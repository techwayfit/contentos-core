namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Taxonomy;

using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;

/// <summary>
/// Universal tagging system.
/// </summary>
public class TagRow : BaseTenantEntity
{
    public string TagName { get; set; } = string.Empty;
    public string Taxonomy { get; set; } = string.Empty;
}
