namespace TechWayFit.ContentOS.Kernel.Primitives;

/// <summary>
/// Platform-wide value object representing a unique tenant identifier.
/// Used across all bounded contexts for multi-tenancy.
/// </summary>
public record TenantId(Guid Value)
{
    public static TenantId New() => new(Guid.NewGuid());
    public static TenantId Empty => new(Guid.Empty);
    
    public override string ToString() => Value.ToString();
}

/// <summary>
/// Platform-wide value object representing a unique site identifier within a tenant.
/// Used for multi-site/brand support within a single tenant.
/// </summary>
public record SiteId(Guid Value)
{
    public static SiteId New() => new(Guid.NewGuid());
    public static SiteId Empty => new(Guid.Empty);
    
    public override string ToString() => Value.ToString();
}

/// <summary>
/// Platform-wide value object representing a unique user identifier.
/// Used across all bounded contexts for user identification.
/// </summary>
public record UserId(Guid Value)
{
    public static UserId New() => new(Guid.NewGuid());
    public static UserId Empty => new(Guid.Empty);
    
    public override string ToString() => Value.ToString();
}

/// <summary>
/// Platform-wide value object representing a generic entity identifier.
/// Can be used for any domain entity that needs a unique ID.
/// </summary>
public record EntityId(Guid Value)
{
    public static EntityId New() => new(Guid.NewGuid());
    public static EntityId Empty => new(Guid.Empty);
    
    public override string ToString() => Value.ToString();
}
