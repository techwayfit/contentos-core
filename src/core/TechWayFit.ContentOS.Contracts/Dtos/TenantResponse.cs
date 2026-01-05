using TechWayFit.ContentOS.Tenancy.Domain;

namespace TechWayFit.ContentOS.Contracts.Dtos;

/// <summary>
/// Tenant details response
/// </summary>
public record TenantResponse(
    Guid Id,
    string Key,
    string Name,
    TenantStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
