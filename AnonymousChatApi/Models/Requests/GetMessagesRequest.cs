namespace AnonymousChatApi.Models.Requests;

[Serializable]
public sealed record GetMessagesRequest(Ulid ChatId);