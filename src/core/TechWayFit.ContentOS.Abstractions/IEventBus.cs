namespace TechWayFit.ContentOS.Abstractions;

/// <summary>
/// Event bus abstraction for publishing and subscribing to domain events
/// </summary>
public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class;
    void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class;
}
