namespace TechWayFit.ContentOS.Contracts.Dtos.Groups;

/// <summary>
/// Group response
/// </summary>
public record GroupResponse(
    Guid Id,
    Guid TenantId,
    string Name,
    DateTime CreatedOn,
    DateTime UpdatedOn);
