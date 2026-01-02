namespace TechWayFit.ContentOS.AI;

/// <summary>
/// Provider-agnostic AI capability abstraction
/// Implementations should integrate with specific AI providers (OpenAI, Azure, etc.)
/// </summary>
public interface IAiProvider
{
    string ProviderName { get; }
    Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default);
}
