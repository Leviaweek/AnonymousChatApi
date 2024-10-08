namespace AnonymousChatApi.Models.Dtos;

[Serializable]
public sealed record ChatDto(Ulid Id, string Name, DateTimeOffset CreatedAt);