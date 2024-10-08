using AnonymousChatApi.Abstractions;
using AnonymousChatApi.Models.Dtos;

namespace AnonymousChatApi.Models.Events;

public sealed class UserJoinEvent(UserJoinDto dto): EventBase, IEvent<UserJoinDto>
{
    public UserJoinDto Body { get; } = dto;
    public override string EventName  => "user-join";
}