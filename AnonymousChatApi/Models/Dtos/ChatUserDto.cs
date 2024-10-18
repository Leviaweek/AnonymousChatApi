namespace AnonymousChatApi.Models.Dtos;

[Serializable]
public sealed record ChatUserDto(long UserId, string UserName);