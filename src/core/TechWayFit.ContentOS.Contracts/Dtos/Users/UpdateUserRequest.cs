namespace TechWayFit.ContentOS.Contracts.Dtos.Users;

/// <summary>
/// Request to update a user
/// </summary>
public record UpdateUserRequest(
    string? Email,
    string? DisplayName);
