using AnonymousChatApi.Abstractions;

namespace AnonymousChatApi.Models;

public sealed class NewMessageEvent(ChatMessage body)
    : EventBase, IEvent<ChatMessage>
{
    public ChatMessage Body { get; } = body;
    public override string EventName => "new-message";
}