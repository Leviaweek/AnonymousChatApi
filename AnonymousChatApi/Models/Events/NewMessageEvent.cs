using AnonymousChatApi.Abstractions;
using AnonymousChatApi.Models.Dtos;

namespace AnonymousChatApi.Models.Events;

public sealed class NewMessageEvent(MessageDto body)
    : EventBase, IEvent<MessageDto>
{
    public MessageDto Body { get; } = body;
    public override string EventName => "new-message";
}