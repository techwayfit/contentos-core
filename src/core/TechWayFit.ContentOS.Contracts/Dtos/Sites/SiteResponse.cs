namespace TechWayFit.ContentOS.Contracts.Dtos.Sites;

/// <summary>
/// Site response
/// </summary>
public record SiteResponse(
    Guid Id,
    Guid TenantId,
    string Name,
    string HostName,
    string DefaultLocale,
    DateTime CreatedOn,
    DateTime UpdatedOn);
