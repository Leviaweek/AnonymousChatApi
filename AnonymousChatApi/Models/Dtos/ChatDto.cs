namespace AnonymousChatApi.Models.Dtos;

[Serializable]
public sealed record ChatDto(long Id, string Name, DateTimeOffset CreatedAt);