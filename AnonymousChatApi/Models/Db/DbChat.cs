namespace AnonymousChatApi.Models.Db;

[Serializable]
public sealed record DbChat(Ulid Id, string Name, DateTimeOffset CreatedAt);
