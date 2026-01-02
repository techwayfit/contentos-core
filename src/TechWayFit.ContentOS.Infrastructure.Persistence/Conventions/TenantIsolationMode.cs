namespace TechWayFit.ContentOS.Infrastructure.Persistence.Conventions;

/// <summary>
/// Defines how tenant data is isolated in the database
/// </summary>
public enum TenantIsolationMode
{
    /// <summary>
    /// All tenants share tables with TenantId column + global query filters
    /// Most common for SaaS applications
    /// </summary>
    SharedTables = 0,

    /// <summary>
    /// Each tenant gets its own database schema
    /// Better isolation, more complex migrations
    /// </summary>
    SeparateSchemas = 1,

    /// <summary>
    /// Each tenant gets its own database
    /// Maximum isolation, highest operational complexity
    /// </summary>
    SeparateDatabases = 2
}
