using AnonymousChatApi.Abstractions;
using AnonymousChatApi.Models.Dtos;

namespace AnonymousChatApi.Models.Events;

public sealed class NewMessageEvent(ChatMessageDto body)
    : EventBase, IEvent<ChatMessageDto>
{
    public ChatMessageDto Body { get; } = body;
    public override string EventName => "new-message";
}