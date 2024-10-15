namespace AnonymousChatApi.Models.Dtos;

[Serializable]
public sealed record MessageDto(
    long ChatId,
    long Id,
    DateTimeOffset TimeStamp,
    long SenderId,
    NotifyMessageDto? NotifyMessage,
    TextMessageDto? TextMessage);