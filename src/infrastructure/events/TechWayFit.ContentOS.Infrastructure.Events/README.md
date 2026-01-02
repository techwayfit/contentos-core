# Infrastructure.Events

## Purpose
Provides concrete implementations of event bus abstractions for domain event publishing and subscription.

## Location in Architecture
**Infrastructure Layer** - Implements `IEventBus` from Abstractions.

## Contains
- `InMemoryEventBus` - Thread-safe, in-memory event bus for single-instance deployments
- DI registration extensions

## Usage

### Registration
```csharp
// In Program.cs or startup
builder.Services.AddInMemoryEventBus();
```

### Publishing Events
```csharp
public class MyService
{
    private readonly IEventBus _eventBus;

    public MyService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task DoSomething()
    {
        // Publish domain event
        await _eventBus.PublishAsync(new ContentCreatedEvent(contentId));
    }
}
```

### Subscribing to Events
```csharp
// Subscribe in application startup
eventBus.Subscribe<ContentCreatedEvent>(async @event =>
{
    // Handle event
    await HandleContentCreated(@event);
});
```

## Implementation Notes
- **InMemoryEventBus** is suitable for single-instance deployments
- Events are delivered synchronously in-process
- Not suitable for distributed systems or multi-instance deployments
- For production distributed scenarios, replace with:
  - Azure Service Bus
  - RabbitMQ
  - Apache Kafka
  - AWS EventBridge
  
## Architectural Compliance
✅ Infrastructure implementation of Abstractions contract  
✅ No domain logic  
✅ Swappable for other event bus implementations  
✅ Separated from Kernel (domain primitives)  
