namespace AnonymousChatApi.Models.Dtos;

[Serializable]
public sealed record UserJoinDto(long UserId, long ChatId, DateTimeOffset TimeStamp);