namespace TechWayFit.ContentOS.Contracts.Dtos.Sites;

/// <summary>
/// Request to create a new site
/// </summary>
public record CreateSiteRequest(
    string Name,
    string HostName,
    string DefaultLocale);
