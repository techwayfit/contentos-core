using System.Collections.Concurrent;
using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Infrastructure.Events;

/// <summary>
/// In-memory event bus implementation for domain events.
/// Thread-safe implementation supporting multiple subscribers per event type.
/// Suitable for single-instance deployments; use message broker for distributed scenarios.
/// </summary>
public class InMemoryEventBus : IEventBus
{
    private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();
    private readonly object _lock = new();

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : class
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        var eventType = typeof(TEvent);

        if (!_handlers.TryGetValue(eventType, out var handlers))
            return; // No handlers registered for this event type

        // Create a snapshot of handlers to avoid modification during iteration
        Delegate[] handlersCopy;
        lock (_lock)
        {
            handlersCopy = handlers.ToArray();
        }

        // Execute all handlers
        foreach (var handler in handlersCopy)
        {
            if (handler is Func<TEvent, Task> asyncHandler)
            {
                await asyncHandler(@event);
            }
        }
    }

    public void Subscribe<TEvent>(Func<TEvent, Task> handler) 
        where TEvent : class
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        var eventType = typeof(TEvent);

        lock (_lock)
        {
            if (!_handlers.TryGetValue(eventType, out var handlers))
            {
                handlers = new List<Delegate>();
                _handlers[eventType] = handlers;
            }

            handlers.Add(handler);
        }
    }
}
