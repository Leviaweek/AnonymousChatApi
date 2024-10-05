namespace AnonymousChatApi.Models;

[Serializable]
public sealed record ChatMessage(
    Ulid ChatId,
    Ulid SenderId,
    Ulid Id,
    string Text,
    DateTimeOffset TimeStamp);