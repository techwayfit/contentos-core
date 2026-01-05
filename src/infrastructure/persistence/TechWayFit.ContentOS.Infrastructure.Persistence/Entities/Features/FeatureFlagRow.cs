namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Features;

using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;

/// <summary>
/// Feature flags for gradual rollouts and A/B testing.
/// </summary>
public class FeatureFlagRow : BaseTenantEntity
{
    public string FeatureKey { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string RulesJson { get; set; } = "{}";
}
