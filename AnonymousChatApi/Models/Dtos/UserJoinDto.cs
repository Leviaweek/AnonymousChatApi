namespace AnonymousChatApi.Models.Dtos;

[Serializable]
public sealed record UserJoinDto(Ulid UserId, Ulid ChatId, DateTimeOffset TimeStamp);