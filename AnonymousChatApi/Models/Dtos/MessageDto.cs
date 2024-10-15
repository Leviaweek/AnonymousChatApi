namespace AnonymousChatApi.Models.Dtos;

[Serializable]
public sealed record MessageDto(
    long ChatId,
    long SenderId,
    long? Id,
    DateTimeOffset TimeStamp,
    NotifyMessageDto? NotifyMessage,
    TextMessageDto? TextMessage);