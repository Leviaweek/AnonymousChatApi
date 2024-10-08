using System.Threading.Channels;

namespace AnonymousChatApi.Models;

public sealed record UserSubscription<T>(Channel<T> Channel, Action Unsubscribe): IDisposable
{
    public void Dispose() => Unsubscribe();
}