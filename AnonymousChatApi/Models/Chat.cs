namespace AnonymousChatApi.Models;

[Serializable]
public sealed record Chat(Ulid Id, string Name, DateTimeOffset CreatedAt);
