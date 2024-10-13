using System.Collections.Concurrent;
using System.Text.Json;
using AnonymousChatApi.Abstractions;
using AnonymousChatApi.Models.Dtos;
using AnonymousChatApi.Models.Events;

namespace AnonymousChatApi.Services;

public sealed class EventHandler
{
    private readonly ConcurrentDictionary<Ulid, UserEventHandler<EventBase>> _eventHandlers = [];
    
    public async Task SubscribeOnEventAsync(Ulid userId,
        Func<string, string, CancellationToken, Task> action, CancellationToken cancellationToken)
    {
        if (!_eventHandlers.TryGetValue(userId, out var eventHandler))
        {
            eventHandler = new UserEventHandler<EventBase>(() => _eventHandlers.TryRemove(userId, out _));
            _eventHandlers.TryAdd(userId, eventHandler);
        }

        using var session = eventHandler.AddSession();
        
        await foreach (var message in session.Channel.Reader.ReadAllAsync(cancellationToken))
        {
            var serialized = JsonSerializer.Serialize(message, message.GetType(), Options.JsonOptions);
            await action(message.EventName, serialized, cancellationToken);
        }
    }
    
    public async Task OnEventAsync<T>(Ulid userId, T @event, CancellationToken cancellationToken) where T: EventBase
    {
        if (!_eventHandlers.TryGetValue(userId, out var handler))
            return;

        await handler.BroadcastEventAsync(@event, cancellationToken);
    }
}