namespace AnonymousChatApi.Models.Dtos;

[Serializable]
public sealed record UserDto(long Id, string Login, DateTimeOffset TimeStamp);