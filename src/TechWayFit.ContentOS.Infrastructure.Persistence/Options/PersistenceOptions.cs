namespace TechWayFit.ContentOS.Infrastructure.Persistence.Options;

/// <summary>
/// Provider-agnostic persistence configuration options
/// </summary>
public class PersistenceOptions
{
    /// <summary>
    /// Name of the connection string in configuration (e.g., "PostgreSQL", "ContentOsDb")
    /// </summary>
    public string ConnectionStringName { get; set; } = "PostgreSQL";

    /// <summary>
    /// Default database schema (provider-specific behavior)
    /// </summary>
    public string? DefaultSchema { get; set; }

    /// <summary>
    /// Command timeout in seconds
    /// </summary>
    public int CommandTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Maximum retry count for transient failures
    /// </summary>
    public int MaxRetryCount { get; set; } = 3;

    /// <summary>
    /// Maximum delay between retries
    /// </summary>
    public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Enable detailed query logging (development only)
    /// </summary>
    public bool EnableSensitiveDataLogging { get; set; } = false;

    /// <summary>
    /// Enable detailed errors (development only)
    /// </summary>
    public bool EnableDetailedErrors { get; set; } = false;
}
