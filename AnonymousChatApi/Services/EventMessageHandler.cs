using System.Collections.Concurrent;
using System.Text.Json;
using AnonymousChatApi.Abstractions;
using AnonymousChatApi.Models;
using AnonymousChatApi.Models.Db;

namespace AnonymousChatApi.Services;

public sealed class EventMessageHandler
{
    private readonly ConcurrentDictionary<Ulid, UserEventHandler<EventBase>> _eventHandlers = [];
    
    public async Task SubscribeOnMessageAsync(Ulid userId,
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
            var serialized = JsonSerializer.Serialize(message, message.GetType(),
                JsonOptions.Options);
            await action(message.EventName, serialized, cancellationToken);
        }
    }

    public async Task OnNewMessageAsync(Ulid userId, DbChatMessage message, CancellationToken cancellationToken)
    {
        if (!_eventHandlers.TryGetValue(userId, out var handler))
            return;

        var newMessageEvent = new NewMessageEvent(message);

        await handler.BroadcastEventAsync(newMessageEvent, cancellationToken);
    }
}