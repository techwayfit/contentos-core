namespace TechWayFit.ContentOS.Abstractions;

/// <summary>
/// Handler interface for domain events
/// </summary>
public interface IEventHandler<in TEvent> where TEvent : class
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}
