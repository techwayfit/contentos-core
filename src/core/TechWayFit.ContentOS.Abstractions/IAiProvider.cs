namespace TechWayFit.ContentOS.Abstractions;

/// <summary>
/// Provider-agnostic AI capability abstraction
/// Implementations should integrate with specific AI providers (OpenAI, Azure, etc.)
/// Port/contract - implementations are provider-specific
/// </summary>
public interface IAiProvider
{
    string ProviderName { get; }
    Task<string> CompleteAsync(string prompt, CancellationToken cancellationToken = default);
}
