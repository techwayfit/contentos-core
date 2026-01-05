using Microsoft.AspNetCore.Mvc;
using TechWayFit.ContentOS.Kernel.Security;
using TechWayFit.ContentOS.Api.Security;
using TechWayFit.ContentOS.Tenancy.Application;
using TechWayFit.ContentOS.Tenancy.Domain;
using TechWayFit.ContentOS.Contracts.Dtos;

namespace TechWayFit.ContentOS.Api.Controllers;

/// <summary>
/// Admin endpoints for tenant management (SuperAdmin only)
/// </summary>
[RequirePermissions(AdminPermissions.SuperAdmin)]
[ApiController]
[Route("api/admin/tenants")]
public class AdminTenantsController : ControllerBase
{
    private readonly CreateTenantUseCase _createTenant;
    private readonly UpdateTenantUseCase _updateTenant;
    private readonly GetTenantUseCase _getTenant;
    private readonly ListTenantsUseCase _listTenants;

    public AdminTenantsController(
        CreateTenantUseCase createTenant,
        UpdateTenantUseCase updateTenant,
        GetTenantUseCase getTenant,
        ListTenantsUseCase listTenants)
    {
        _createTenant = createTenant;
        _updateTenant = updateTenant;
        _getTenant = getTenant;
        _listTenants = listTenants;
    }

    /// <summary>
    /// Create a new tenant
    /// </summary>
    [HttpPost]
    [RequirePermissions(AdminPermissions.TenantManage)]
    public async Task<IActionResult> CreateTenant(
        [FromBody] CreateTenantRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = await _createTenant.ExecuteAsync(request.Key, request.Name, cancellationToken);

        return Ok(new CreateTenantResponse(tenantId));
    }

    /// <summary>
    /// Update an existing tenant
    /// </summary>
    [HttpPut("{id}")]
    [RequirePermissions(AdminPermissions.TenantManage)]
    public async Task<IActionResult> UpdateTenant(
        Guid id,
        [FromBody] UpdateTenantRequest request,
        CancellationToken cancellationToken)
    {
        await _updateTenant.ExecuteAsync(id, request.Name, request.Status, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Get a tenant by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTenant(Guid id, CancellationToken cancellationToken)
    {
        var tenant = await _getTenant.ExecuteAsync(id, cancellationToken);

        if (tenant == null)
        {
            return NotFound(new { error = $"Tenant {id} not found" });
        }

        return Ok(new TenantResponse(
            tenant.Id,
            tenant.Key,
            tenant.Name,
            tenant.Status,
            tenant.CreatedAt,
            tenant.UpdatedAt));
    }

    /// <summary>
    /// List all tenants
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListTenants(
        [FromQuery] TenantStatus? status,
        CancellationToken cancellationToken)
    {
        var tenants = await _listTenants.ExecuteAsync(status, cancellationToken);

        var response = tenants.Select(t => new TenantResponse(
            t.Id,
            t.Key,
            t.Name,
            t.Status,
            t.CreatedAt,
            t.UpdatedAt));

        return Ok(response);
    }
}
