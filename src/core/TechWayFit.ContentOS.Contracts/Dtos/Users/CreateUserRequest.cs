namespace TechWayFit.ContentOS.Contracts.Dtos.Users;

/// <summary>
/// Request to create a new user
/// </summary>
public record CreateUserRequest(
    string Email,
    string DisplayName);
