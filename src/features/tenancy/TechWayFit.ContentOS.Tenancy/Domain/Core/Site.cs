using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Tenancy.Domain.Core;

/// <summary>
/// Site domain entity - Pure POCO
/// Represents a multi-site within a tenant (different hostnames, locales, delivery scopes)
/// </summary>
public sealed class Site
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = default!;
    public string HostName { get; set; } = default!;
    public string DefaultLocale { get; set; } = default!;
    public AuditInfo Audit { get; set; } = new();
}
