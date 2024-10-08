using AnonymousChatApi.Models;
using AnonymousChatApi.Models.Db;
using AnonymousChatApi.Models.Dtos;

namespace AnonymousChatApi;

public static class TypeMapper
{
    public static ChatMessageDto ToDto(this DbChatMessage message) =>
        new(ChatId: message.ChatId,
            Id: message.Id,
            Text: message.Text,
            TimeStamp: message.TimeStamp);

    public static DbChatMessage ToDb(this ChatMessageDto dto, Ulid id) => new(
        SenderId: id,
        ChatId: dto.ChatId,
        Id: dto.Id,
        Text: dto.Text,
        TimeStamp: dto.TimeStamp);

    public static ChatDto ToDto(this DbChat chat) => new(
        Id: chat.Id,
        Name: chat.Name,
        CreatedAt: chat.CreatedAt);
}