using Microsoft.AspNetCore.Mvc;
using TechWayFit.ContentOS.Abstractions.Security;
using TechWayFit.ContentOS.Kernel.Security;
using TechWayFit.ContentOS.Tenancy.Application;
using TechWayFit.ContentOS.Tenancy.Domain;

namespace TechWayFit.ContentOS.Api.Controllers;

/// <summary>
/// Admin endpoints for tenant management (SuperAdmin only)
/// </summary>
[ApiController]
[Route("api/admin/tenants")]
public class AdminTenantsController : ControllerBase
{
    private readonly IPolicyEvaluator _policyEvaluator;
    private readonly CreateTenantUseCase _createTenant;
    private readonly UpdateTenantUseCase _updateTenant;
    private readonly GetTenantUseCase _getTenant;
    private readonly ListTenantsUseCase _listTenants;

    public AdminTenantsController(
        IPolicyEvaluator policyEvaluator,
        CreateTenantUseCase createTenant,
        UpdateTenantUseCase updateTenant,
        GetTenantUseCase getTenant,
        ListTenantsUseCase listTenants)
    {
        _policyEvaluator = policyEvaluator;
        _createTenant = createTenant;
        _updateTenant = updateTenant;
        _getTenant = getTenant;
        _listTenants = listTenants;
    }

    /// <summary>
    /// Create a new tenant
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateTenant(
        [FromBody] CreateTenantRequest request,
        CancellationToken cancellationToken)
    {
        // Require SuperAdmin permissions
        await _policyEvaluator.RequireAsync(AdminPermissions.SuperAdmin, cancellationToken);
        await _policyEvaluator.RequireAsync(AdminPermissions.TenantManage, cancellationToken);

        var tenantId = await _createTenant.ExecuteAsync(request.Key, request.Name, cancellationToken);

        return Ok(new CreateTenantResponse(tenantId));
    }

    /// <summary>
    /// Update an existing tenant
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTenant(
        Guid id,
        [FromBody] UpdateTenantRequest request,
        CancellationToken cancellationToken)
    {
        // Require SuperAdmin permissions
        await _policyEvaluator.RequireAsync(AdminPermissions.SuperAdmin, cancellationToken);
        await _policyEvaluator.RequireAsync(AdminPermissions.TenantManage, cancellationToken);

        await _updateTenant.ExecuteAsync(id, request.Name, request.Status, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Get a tenant by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTenant(Guid id, CancellationToken cancellationToken)
    {
        // Require SuperAdmin permissions
        await _policyEvaluator.RequireAsync(AdminPermissions.SuperAdmin, cancellationToken);

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
        // Require SuperAdmin permissions
        await _policyEvaluator.RequireAsync(AdminPermissions.SuperAdmin, cancellationToken);

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

// DTOs
public record CreateTenantRequest(string Key, string Name);
public record UpdateTenantRequest(string Name, TenantStatus Status);
public record CreateTenantResponse(Guid Id);
public record TenantResponse(
    Guid Id,
    string Key,
    string Name,
    TenantStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
