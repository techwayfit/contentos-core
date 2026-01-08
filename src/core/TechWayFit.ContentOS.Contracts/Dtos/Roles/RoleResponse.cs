namespace TechWayFit.ContentOS.Contracts.Dtos.Roles;

/// <summary>
/// Role response
/// </summary>
public record RoleResponse(
    Guid Id,
    Guid TenantId,
    string Name,
    DateTime CreatedOn,
    DateTime UpdatedOn);
