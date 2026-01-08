namespace TechWayFit.ContentOS.Contracts.Dtos.Roles;

/// <summary>
/// Request to assign/remove role to/from user
/// </summary>
public record AssignRoleRequest(Guid UserId, Guid RoleId);
