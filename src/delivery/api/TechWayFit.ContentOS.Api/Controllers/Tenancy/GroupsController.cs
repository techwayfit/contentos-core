using Microsoft.AspNetCore.Mvc;
using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Contracts.Dtos.Groups;
using TechWayFit.ContentOS.Tenancy.Application.Groups;

namespace TechWayFit.ContentOS.Api.Controllers.Tenancy;

/// <summary>
/// Endpoints for group management within a tenant
/// </summary>
[ApiController]
[Route("api/tenancy/groups")]
public class GroupsController : ControllerBase
{
    private readonly ICurrentTenantProvider _tenantProvider;
    private readonly CreateGroupUseCase _createGroup;
    private readonly UpdateGroupUseCase _updateGroup;
    private readonly DeleteGroupUseCase _deleteGroup;
    private readonly GetGroupUseCase _getGroup;
    private readonly ListGroupsUseCase _listGroups;
    private readonly AddUserToGroupUseCase _addUserToGroup;
    private readonly RemoveUserFromGroupUseCase _removeUserFromGroup;

    public GroupsController(
        ICurrentTenantProvider tenantProvider,
        CreateGroupUseCase createGroup,
        UpdateGroupUseCase updateGroup,
        DeleteGroupUseCase deleteGroup,
        GetGroupUseCase getGroup,
        ListGroupsUseCase listGroups,
        AddUserToGroupUseCase addUserToGroup,
        RemoveUserFromGroupUseCase removeUserFromGroup)
    {
        _tenantProvider = tenantProvider;
        _createGroup = createGroup;
        _updateGroup = updateGroup;
        _deleteGroup = deleteGroup;
        _getGroup = getGroup;
        _listGroups = listGroups;
        _addUserToGroup = addUserToGroup;
        _removeUserFromGroup = removeUserFromGroup;
    }

    /// <summary>
    /// Create a new group
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateGroup(
        [FromBody] CreateGroupRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _createGroup.ExecuteAsync(
            tenantId,
            request.Name,
            cancellationToken);

        return result.Match<IActionResult>(
            groupId => CreatedAtAction(nameof(GetGroup), new { id = groupId }, new { id = groupId }),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Update a group
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGroup(
        Guid id,
        [FromBody] UpdateGroupRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _updateGroup.ExecuteAsync(
            id,
            tenantId,
            request.Name,
            cancellationToken);

        return result.Match<IActionResult>(
            success => NoContent(),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Delete a group
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGroup(Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _deleteGroup.ExecuteAsync(id, tenantId, cancellationToken);

        return result.Match<IActionResult>(
            success => NoContent(),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Get a group by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGroup(Guid id, CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _getGroup.ExecuteAsync(id, tenantId, cancellationToken);

        return result.Match<IActionResult>(
            group => Ok(new GroupResponse(
                group.Id,
                group.TenantId,
                group.Name,
                group.Audit.CreatedOn,
                group.Audit.UpdatedOn)),
            error => NotFound(new { error }));
    }

    /// <summary>
    /// List all groups for the current tenant
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListGroups(CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _listGroups.ExecuteAsync(tenantId, cancellationToken);

        return result.Match<IActionResult>(
            groups => Ok(groups.Select(g => new GroupResponse(
                g.Id,
                g.TenantId,
                g.Name,
                g.Audit.CreatedOn,
                g.Audit.UpdatedOn))),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Add a user to a group
    /// </summary>
    [HttpPost("add-member")]
    public async Task<IActionResult> AddUserToGroup(
        [FromBody] GroupMemberRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _addUserToGroup.ExecuteAsync(
            request.UserId,
            request.GroupId,
            tenantId,
            cancellationToken);

        return result.Match<IActionResult>(
            success => NoContent(),
            error => BadRequest(new { error }));
    }

    /// <summary>
    /// Remove a user from a group
    /// </summary>
    [HttpPost("remove-member")]
    public async Task<IActionResult> RemoveUserFromGroup(
        [FromBody] GroupMemberRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantProvider.TenantId;

        var result = await _removeUserFromGroup.ExecuteAsync(
            request.UserId,
            request.GroupId,
            tenantId,
            cancellationToken);

        return result.Match<IActionResult>(
            success => NoContent(),
            error => BadRequest(new { error }));
    }
}
