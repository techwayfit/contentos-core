namespace TechWayFit.ContentOS.Contracts.Dtos.Sites;

/// <summary>
/// Request to update a site
/// </summary>
public record UpdateSiteRequest(
    string? Name,
    string? HostName,
    string? DefaultLocale);
