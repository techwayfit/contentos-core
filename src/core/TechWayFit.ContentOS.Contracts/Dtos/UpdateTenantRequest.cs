using TechWayFit.ContentOS.Tenancy.Domain;

namespace TechWayFit.ContentOS.Contracts.Dtos;

/// <summary>
/// Request to update an existing tenant
/// </summary>
public record UpdateTenantRequest(string Name, TenantStatus Status);
