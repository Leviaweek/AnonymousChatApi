namespace AnonymousChatApi.Models.Requests;

[Serializable]
public sealed record JoinChatRequest(long UserId, long ChatId);