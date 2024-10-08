namespace AnonymousChatApi.Models.Db;

[Serializable]
public sealed record DbChatMessage(
    Ulid SenderId,
    Ulid ChatId,
    Ulid Id,
    string Text,
    DateTimeOffset TimeStamp);