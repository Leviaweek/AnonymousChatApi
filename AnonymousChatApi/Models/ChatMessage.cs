namespace AnonymousChatApi.Models;

[Serializable]
public sealed record ChatMessage(
    Ulid ChatId,
    Ulid Id,
    string Text,
    DateTimeOffset TimeStamp);