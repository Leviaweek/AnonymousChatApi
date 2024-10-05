using AnonymousChatApi.Models;

namespace AnonymousChatApi;

public static class TypeMapper
{
    public static ChatMessageDto ToDto(this ChatMessage message) =>
        new(SenderId: message.SenderId,
            ChatId: message.ChatId,
            MessageId: message.Id,
            Text: message.Text,
            TimeStamp: message.TimeStamp);
}