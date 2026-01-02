namespace TechWayFit.ContentOS.Abstractions;

/// <summary>
/// Contract for running database migrations
/// Implementations are provider-specific (e.g., EF Core migrations for Postgres)
/// Port/contract - belongs in Abstractions
/// </summary>
public interface IMigrationRunner
{
    /// <summary>
    /// Apply all pending migrations to bring database to latest version
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task MigrateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get list of pending migrations that haven't been applied yet
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<IEnumerable<string>> GetPendingMigrationsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get list of all migrations that have been applied
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<IEnumerable<string>> GetAppliedMigrationsAsync(CancellationToken cancellationToken = default);
}
