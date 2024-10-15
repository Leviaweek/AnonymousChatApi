namespace AnonymousChatApi.Models.Requests;

[Serializable]
public sealed record CreateChatRequest(long UserId, string Name);