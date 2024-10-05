namespace AnonymousChatApi.Models;

[Serializable]
public sealed record ChatMessageDto(
    Ulid SenderId,
    Ulid ChatId,
    Ulid MessageId,
    string Text,
    DateTimeOffset TimeStamp);