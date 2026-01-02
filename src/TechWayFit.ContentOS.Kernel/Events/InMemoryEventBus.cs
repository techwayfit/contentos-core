using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Kernel.Events;

/// <summary>
/// In-memory event bus implementation for domain events
/// </summary>
public class InMemoryEventBus : IEventBus
{
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class
    {
        // Placeholder implementation
        return Task.CompletedTask;
    }

    public void Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class
    {
        // Placeholder implementation
    }
}
