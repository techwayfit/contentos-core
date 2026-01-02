namespace TechWayFit.ContentOS.Contracts.Dtos;

/// <summary>
/// Tenant configuration data transfer object
/// </summary>
public record TenantDto
{
    public string TenantId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public List<SiteDto> Sites { get; init; } = new();
}

/// <summary>
/// Site configuration within a tenant
/// </summary>
public record SiteDto
{
    public string SiteId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public List<string> Environments { get; init; } = new();
}
