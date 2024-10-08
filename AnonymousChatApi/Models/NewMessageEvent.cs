using AnonymousChatApi.Abstractions;
using AnonymousChatApi.Models.Db;

namespace AnonymousChatApi.Models;

public sealed class NewMessageEvent(DbChatMessage body)
    : EventBase, IEvent<DbChatMessage>
{
    public DbChatMessage Body { get; } = body;
    public override string EventName => "new-message";
}