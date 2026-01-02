namespace TechWayFit.ContentOS.AI;

/// <summary>
/// Optional AI orchestration service - provider agnostic
/// AI capabilities are optional and should not be required for core platform functionality
/// </summary>
public interface IAiOrchestrationService
{
    Task<object> GenerateAsync(string prompt, CancellationToken cancellationToken = default);
    Task<object> AnalyzeAsync(string content, CancellationToken cancellationToken = default);
}
