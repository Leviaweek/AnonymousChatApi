using System.Collections.Concurrent;
using System.Threading.Channels;
using AnonymousChatApi.Models;

namespace AnonymousChatApi.Services;

public sealed class UserEventHandler<T>(Action selfDestruct)
{
    private readonly ConcurrentDictionary<Ulid, Channel<T>> _sessions = [];

    public UserSubscription<T> AddSession()
    {
        var id = Ulid.NewUlid();
        var channel = Channel.CreateBounded<T>(new BoundedChannelOptions(15)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.DropOldest
        });
        _sessions.TryAdd(id, channel);
        Console.WriteLine("User added: {0}", id);
        return new UserSubscription<T>(channel, () =>
        {
            _sessions.TryRemove(id, out _);
            Console.WriteLine("User removed: {0}", id);
            if (_sessions.IsEmpty)
                selfDestruct();
        });
    }

    public async Task BroadcastEventAsync(T @event, CancellationToken cancellationToken)
    {
        foreach (var (_, session) in _sessions)
            await session.Writer.WriteAsync(@event, cancellationToken);
    }
}