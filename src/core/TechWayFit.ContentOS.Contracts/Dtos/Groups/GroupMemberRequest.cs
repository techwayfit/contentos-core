namespace TechWayFit.ContentOS.Contracts.Dtos.Groups;

/// <summary>
/// Request to add/remove user to/from group
/// </summary>
public record GroupMemberRequest(Guid UserId, Guid GroupId);
