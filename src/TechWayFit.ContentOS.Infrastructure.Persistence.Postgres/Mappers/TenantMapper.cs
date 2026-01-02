using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;
using TechWayFit.ContentOS.Tenancy.Domain;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Mappers;

/// <summary>
/// Mapper between Tenant domain entity and TenantRow
/// </summary>
public static class TenantMapper
{
    public static TenantRow ToRow(Tenant domain)
    {
        return new TenantRow
        {
            Id = domain.Id,
            Key = domain.Key,
            Name = domain.Name,
            Status = (int)domain.Status,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static Tenant ToDomain(TenantRow row)
    {
        // Use reflection to create domain entity (since constructor is private)
        var tenant = (Tenant)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(Tenant));

        typeof(Tenant).GetProperty(nameof(Tenant.Id))!.SetValue(tenant, row.Id);
        typeof(Tenant).GetProperty(nameof(Tenant.Key))!.SetValue(tenant, row.Key);
        typeof(Tenant).GetProperty(nameof(Tenant.Name))!.SetValue(tenant, row.Name);
        typeof(Tenant).GetProperty(nameof(Tenant.Status))!.SetValue(tenant, (TenantStatus)row.Status);
        typeof(Tenant).GetProperty(nameof(Tenant.CreatedAt))!.SetValue(tenant, row.CreatedAt);
        typeof(Tenant).GetProperty(nameof(Tenant.UpdatedAt))!.SetValue(tenant, row.UpdatedAt);

        return tenant;
    }
}
