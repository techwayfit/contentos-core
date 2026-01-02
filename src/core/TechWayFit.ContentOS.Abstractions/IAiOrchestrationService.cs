namespace TechWayFit.ContentOS.Abstractions;

/// <summary>
/// Optional AI orchestration service - provider agnostic
/// AI capabilities are optional and should not be required for core platform functionality
/// Port/contract - implementations are provider-specific
/// </summary>
public interface IAiOrchestrationService
{
    Task<object> GenerateAsync(string prompt, CancellationToken cancellationToken = default);
    Task<object> AnalyzeAsync(string content, CancellationToken cancellationToken = default);
}
