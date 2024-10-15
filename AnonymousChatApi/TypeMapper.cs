using AnonymousChatApi.Databases.Models;
using AnonymousChatApi.Models.Dtos;

namespace AnonymousChatApi;

public static class TypeMapper
{
    public static MessageDto ToDto(this MessageBase messageBase) =>
        new(ChatId: messageBase.ChatId,
            Id: messageBase.Id,
            TextMessage: messageBase.TextMessage?.ToDto(),
            NotifyMessage: messageBase.NotifyMessage?.ToDto(),
            TimeStamp: messageBase.TimeStamp,
            SenderId: messageBase.UserId);

    public static TextMessageDto ToDto(this TextMessage textMessage) => new(textMessage.Text);

    public static NotifyMessageDto ToDto(this NotifyMessage notifyMessage) => new(notifyMessage.Type);
    
    public static ChatDto ToDto(this Chat chat) => new(
        Id: chat.Id,
        Name: chat.Name,
        CreatedAt: chat.CreatedAt);

    public static UserDto ToDto(this User user) => new(
        Id: user.Id,
        Login: user.Login,
        TimeStamp: user.TimeStamp);
}