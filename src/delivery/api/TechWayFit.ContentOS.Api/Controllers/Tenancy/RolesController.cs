using Microsoft.AspNetCore.Mvc;
using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Contracts.Dtos.Roles;
using TechWayFit.ContentOS.Tenancy.Application.Roles;

namespace TechWayFit.ContentOS.Api.Controllers.Tenancy;

/// <summary>
/// Endpoints for role management within a tenant
/// </summary>
[ApiController]
[Route("api/tenancy/roles")]
public class RolesController : ControllerBase
{
    private readonly ICurrentTenantProvider _tenantProvider;
    private readonly CreateRoleUseCase _createRole;
    private readonly UpdateRoleUseCase _updateRole;
    private readonly DeleteRoleUseCase _deleteRole;
    private readonly GetRoleUseCase _getRole;
    private readonly ListRolesUseCase _listRoles;
    private readonly AssignRoleToUserUseCase _assignRole;
    private readonly RemoveRoleFromUserUseCase _removeRole;

    public RolesController(
        ICurrentTenantProvider tenantProvider,
        CreateRoleUseCase createRole,
        UpdateRoleUseCase updateRole,
        DeleteRoleUseCase deleteRole,
        GetRoleUseCase getRole,
        ListRolesUseCase listRoles,
        AssignRoleToUserUseCase assignRole,
        RemoveRoleFromUserUseCase removeRole)
    {
        _tenantProvider = tenantProvider;
        _createRole = createRole;
        _updateRole = updateRole;
        _deleteRole = deleteRole;
        _getRole = getRole;
        _listRoles = listRoles;
        _assignRole = assignRole;
        _removeRole = removeRole;
    }

    /// <summary>
    /// Create a new role
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateRole(
        [FromBody] CreateRoleRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _createRole.ExecuteAsync(
            tenantId,
            request.Name,
            cancellationToken);

        return result.Match<IActionResult>(
            roleId => CreatedAtAction(nameof(GetRole), new { id = roleId }, new { id = roleId }),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Update a role
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(
        Guid id,
        [FromBody] UpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _updateRole.ExecuteAsync(
            id,
            tenantId,
            request.Name,
            cancellationToken);

        return result.Match<IActionResult>(
            success => NoContent(),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Delete a role
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _deleteRole.ExecuteAsync(id, tenantId, cancellationToken);

        return result.Match<IActionResult>(
            success => NoContent(),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Get a role by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRole(Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _getRole.ExecuteAsync(id, tenantId, cancellationToken);

        return result.Match<IActionResult>(
            role => Ok(new RoleResponse(
                role.Id,
                role.TenantId,
                role.Name,
                role.Audit.CreatedOn,
                role.Audit.UpdatedOn)),
            error => NotFound(new { error }));
    }

    /// <summary>
    /// List all roles for the current tenant
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListRoles(CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _listRoles.ExecuteAsync(tenantId, cancellationToken);

        return result.Match<IActionResult>(
            roles => Ok(roles.Select(r => new RoleResponse(
                r.Id,
                r.TenantId,
                r.Name,
                r.Audit.CreatedOn,
                r.Audit.UpdatedOn))),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Assign a role to a user
    /// </summary>
    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole(
        [FromBody] AssignRoleRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _assignRole.ExecuteAsync(
            request.UserId,
            request.RoleId,
            tenantId,
            cancellationToken);

        return result.Match<IActionResult>(
            success => NoContent(),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Remove a role from a user
    /// </summary>
    [HttpPost("remove")]
    public async Task<IActionResult> RemoveRole(
        [FromBody] AssignRoleRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _removeRole.ExecuteAsync(
            request.UserId,
            request.RoleId,
            tenantId,
            cancellationToken);

        return result.Match<IActionResult>(
            success => NoContent(),
            error => BadRequest(new { error }));
    }
}
