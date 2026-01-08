namespace TechWayFit.ContentOS.Contracts.Dtos.Users;

/// <summary>
/// User response
/// </summary>
public record UserResponse(
    Guid Id,
    Guid TenantId,
    string Email,
    string DisplayName,
    string Status,
    DateTime CreatedOn,
    DateTime UpdatedOn);
