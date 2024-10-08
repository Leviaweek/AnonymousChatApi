namespace AnonymousChatApi.Models.Dtos;

[Serializable]
public sealed record ChatMessageDto(
    Ulid ChatId,
    Ulid Id,
    string Text,
    DateTimeOffset TimeStamp);