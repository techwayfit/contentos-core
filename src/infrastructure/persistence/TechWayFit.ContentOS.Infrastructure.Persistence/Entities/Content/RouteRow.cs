namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

/// <summary>
/// Delivery routing (friendly URLs → nodes).
/// </summary>
public class RouteRow : BaseTenantSiteEntity
{
    public Guid NodeId { get; set; }
    public string RoutePath { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    
    // Navigation
    public ContentNodeRow? Node { get; set; }
}
