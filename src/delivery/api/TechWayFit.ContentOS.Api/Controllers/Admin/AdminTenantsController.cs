using Microsoft.AspNetCore.Mvc;
using TechWayFit.ContentOS.Kernel.Security;
using TechWayFit.ContentOS.Api.Security;
using TechWayFit.ContentOS.Tenancy.Application.Tenants;
using TechWayFit.ContentOS.Tenancy.Domain;
using TechWayFit.ContentOS.Contracts.Dtos;

namespace TechWayFit.ContentOS.Api.Controllers.Admin;

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
    private readonly SuspendTenantUseCase _suspendTenant;
    private readonly ActivateTenantUseCase _activateTenant;
    private readonly DeleteTenantUseCase _deleteTenant;

    public AdminTenantsController(
        CreateTenantUseCase createTenant,
        UpdateTenantUseCase updateTenant,
        GetTenantUseCase getTenant,
        ListTenantsUseCase listTenants,
        SuspendTenantUseCase suspendTenant,
        ActivateTenantUseCase activateTenant,
        DeleteTenantUseCase deleteTenant)
    {
        _createTenant = createTenant;
        _updateTenant = updateTenant;
        _getTenant = getTenant;
        _listTenants = listTenants;
        _suspendTenant = suspendTenant;
        _activateTenant = activateTenant;
        _deleteTenant = deleteTenant;
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

    /// <summary>
    /// Suspend a tenant (disable access)
    /// </summary>
    [HttpPost("{id}/suspend")]
    [RequirePermissions(AdminPermissions.TenantManage)]
    public async Task<IActionResult> SuspendTenant(Guid id, CancellationToken cancellationToken)
    {
        var result = await _suspendTenant.ExecuteAsync(id, cancellationToken);

        return result.Match<IActionResult>(
            success => NoContent(),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Activate a suspended tenant
    /// </summary>
    [HttpPost("{id}/activate")]
    [RequirePermissions(AdminPermissions.TenantManage)]
    public async Task<IActionResult> ActivateTenant(Guid id, CancellationToken cancellationToken)
    {
        var result = await _activateTenant.ExecuteAsync(id, cancellationToken);

        return result.Match<IActionResult>(
            success => NoContent(),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Delete a tenant permanently (WARNING: Destructive operation)
    /// </summary>
    [HttpDelete("{id}")]
    [RequirePermissions(AdminPermissions.TenantManage)]
    public async Task<IActionResult> DeleteTenant(Guid id, CancellationToken cancellationToken)
    {
        var result = await _deleteTenant.ExecuteAsync(id, cancellationToken);

        return result.Match<IActionResult>(
            success => NoContent(),
            error => BadRequest(new { error }));
    }
}
