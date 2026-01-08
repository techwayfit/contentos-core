using Microsoft.AspNetCore.Mvc;
using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Contracts.Dtos.Sites;
using TechWayFit.ContentOS.Tenancy.Application.Sites;

namespace TechWayFit.ContentOS.Api.Controllers.Tenancy;

/// <summary>
/// Endpoints for site management within a tenant
/// </summary>
[ApiController]
[Route("api/tenancy/sites")]
public class SitesController : ControllerBase
{
    private readonly ICurrentTenantProvider _tenantProvider;
    private readonly CreateSiteUseCase _createSite;
    private readonly UpdateSiteUseCase _updateSite;
    private readonly DeleteSiteUseCase _deleteSite;
    private readonly GetSiteUseCase _getSite;
    private readonly GetSiteByHostNameUseCase _getSiteByHostName;
    private readonly ListSitesUseCase _listSites;

    public SitesController(
        ICurrentTenantProvider tenantProvider,
        CreateSiteUseCase createSite,
        UpdateSiteUseCase updateSite,
        DeleteSiteUseCase deleteSite,
        GetSiteUseCase getSite,
        GetSiteByHostNameUseCase getSiteByHostName,
        ListSitesUseCase listSites)
    {
        _tenantProvider = tenantProvider;
        _createSite = createSite;
        _updateSite = updateSite;
        _deleteSite = deleteSite;
        _getSite = getSite;
        _getSiteByHostName = getSiteByHostName;
        _listSites = listSites;
    }

    /// <summary>
    /// Create a new site
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateSite(
        [FromBody] CreateSiteRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _createSite.ExecuteAsync(
            tenantId,
            request.Name,
            request.HostName,
            request.DefaultLocale,
            cancellationToken);

        return result.Match<IActionResult>(
            siteId => CreatedAtAction(nameof(GetSite), new { id = siteId }, new { id = siteId }),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Update a site
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSite(
        Guid id,
        [FromBody] UpdateSiteRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _updateSite.ExecuteAsync(
            id,
            tenantId,
            request.Name,
            request.HostName,
            request.DefaultLocale,
            cancellationToken);

        return result.Match<IActionResult>(
            success => NoContent(),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Delete a site
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSite(Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _deleteSite.ExecuteAsync(id, tenantId, cancellationToken);

        return result.Match<IActionResult>(
            success => NoContent(),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Get a site by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSite(Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _getSite.ExecuteAsync(id, tenantId, cancellationToken);

        return result.Match<IActionResult>(
            site => Ok(new SiteResponse(
                site.Id,
                site.TenantId,
                site.Name,
                site.HostName,
                site.DefaultLocale,
                site.Audit.CreatedOn,
                site.Audit.UpdatedOn)),
            error => NotFound(new { error }));
    }

    /// <summary>
    /// Get a site by hostname
    /// </summary>
    [HttpGet("by-hostname/{hostName}")]
    public async Task<IActionResult> GetSiteByHostName(string hostName, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _getSiteByHostName.ExecuteAsync(tenantId, hostName, cancellationToken);

        return result.Match<IActionResult>(
            site => Ok(new SiteResponse(
                site.Id,
                site.TenantId,
                site.Name,
                site.HostName,
                site.DefaultLocale,
                site.Audit.CreatedOn,
                site.Audit.UpdatedOn)),
            error => NotFound(new { error }));
    }

    /// <summary>
    /// List all sites for the current tenant
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListSites(CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _listSites.ExecuteAsync(tenantId, cancellationToken);

        return result.Match<IActionResult>(
            sites => Ok(sites.Select(s => new SiteResponse(
                s.Id,
                s.TenantId,
                s.Name,
                s.HostName,
                s.DefaultLocale,
                s.Audit.CreatedOn,
                s.Audit.UpdatedOn))),
            error => BadRequest(new { error }));
    }
}
