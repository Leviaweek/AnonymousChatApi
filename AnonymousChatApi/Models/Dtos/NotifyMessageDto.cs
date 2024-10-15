using AnonymousChatApi.Databases.Models;

namespace AnonymousChatApi.Models.Dtos;

[Serializable]
public sealed record NotifyMessageDto(NotifyType Type);