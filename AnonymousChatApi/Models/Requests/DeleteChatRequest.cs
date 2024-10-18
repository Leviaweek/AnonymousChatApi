namespace AnonymousChatApi.Models.Requests;

[Serializable]
public sealed record DeleteChatRequest(long UserId, long ChatId);