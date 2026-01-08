using Microsoft.AspNetCore.Mvc;
using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Contracts.Dtos.Users;
using TechWayFit.ContentOS.Tenancy.Application.Users;

namespace TechWayFit.ContentOS.Api.Controllers.Tenancy;

/// <summary>
/// Endpoints for user management within a tenant
/// </summary>
[ApiController]
[Route("api/tenancy/users")]
public class UsersController : ControllerBase
{
    private readonly ICurrentTenantProvider _tenantProvider;
    private readonly CreateUserUseCase _createUser;
    private readonly UpdateUserUseCase _updateUser;
    private readonly DeactivateUserUseCase _deactivateUser;
    private readonly ReactivateUserUseCase _reactivateUser;
    private readonly DeleteUserUseCase _deleteUser;
    private readonly GetUserUseCase _getUser;
    private readonly GetUserByEmailUseCase _getUserByEmail;
    private readonly ListUsersUseCase _listUsers;

    public UsersController(
        ICurrentTenantProvider tenantProvider,
        CreateUserUseCase createUser,
        UpdateUserUseCase updateUser,
        DeactivateUserUseCase deactivateUser,
        ReactivateUserUseCase reactivateUser,
        DeleteUserUseCase deleteUser,
        GetUserUseCase getUser,
        GetUserByEmailUseCase getUserByEmail,
        ListUsersUseCase listUsers)
    {
        _tenantProvider = tenantProvider;
        _createUser = createUser;
        _updateUser = updateUser;
        _deactivateUser = deactivateUser;
        _reactivateUser = reactivateUser;
        _deleteUser = deleteUser;
        _getUser = getUser;
        _getUserByEmail = getUserByEmail;
        _listUsers = listUsers;
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _createUser.ExecuteAsync(
            tenantId,
            request.Email,
            request.DisplayName,
            cancellationToken);

        return result.Match<IActionResult>(
            userId => CreatedAtAction(nameof(GetUser), new { id = userId }, new { id = userId }),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Update a user
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _updateUser.ExecuteAsync(
            id,
            tenantId,
            request.Email,
            request.DisplayName,
            cancellationToken);

        return result.Match<IActionResult>(
            success => NoContent(),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Deactivate a user
    /// </summary>
    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> DeactivateUser(Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _deactivateUser.ExecuteAsync(id, tenantId, cancellationToken);

        return result.Match<IActionResult>(
            success => NoContent(),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Reactivate a user
    /// </summary>
    [HttpPost("{id}/reactivate")]
    public async Task<IActionResult> ReactivateUser(Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _reactivateUser.ExecuteAsync(id, tenantId, cancellationToken);

        return result.Match<IActionResult>(
            success => NoContent(),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Delete a user permanently
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _deleteUser.ExecuteAsync(id, tenantId, cancellationToken);

        return result.Match<IActionResult>(
            success => NoContent(),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Get a user by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _getUser.ExecuteAsync(id, tenantId, cancellationToken);

        return result.Match<IActionResult>(
            user => Ok(new UserResponse(
                user.Id,
                user.TenantId,
                user.Email,
                user.DisplayName,
                user.Status,
                user.Audit.CreatedOn,
                user.Audit.UpdatedOn)),
            error => NotFound(new { error }));
    }

    /// <summary>
    /// Get a user by email
    /// </summary>
    [HttpGet("by-email/{email}")]
    public async Task<IActionResult> GetUserByEmail(string email, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _getUserByEmail.ExecuteAsync(tenantId, email, cancellationToken);

        return result.Match<IActionResult>(
            user => Ok(new UserResponse(
                user.Id,
                user.TenantId,
                user.Email,
                user.DisplayName,
                user.Status,
                user.Audit.CreatedOn,
                user.Audit.UpdatedOn)),
            error => NotFound(new { error }));
    }

    /// <summary>
    /// List all users for the current tenant
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListUsers(
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _listUsers.ExecuteAsync(tenantId, status, cancellationToken);

        return result.Match<IActionResult>(
            users => Ok(users.Select(u => new UserResponse(
                u.Id,
                u.TenantId,
                u.Email,
                u.DisplayName,
                u.Status,
                u.Audit.CreatedOn,
                u.Audit.UpdatedOn))),
            error => BadRequest(new { error }));
    }
}
