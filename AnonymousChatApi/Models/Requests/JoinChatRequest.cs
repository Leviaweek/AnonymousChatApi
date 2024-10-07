namespace AnonymousChatApi.Models.Requests;

[Serializable]
public sealed record JoinChatRequest(Ulid ChatId);