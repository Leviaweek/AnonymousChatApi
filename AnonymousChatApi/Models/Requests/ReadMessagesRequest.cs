namespace AnonymousChatApi.Models.Requests;

[Serializable]
public sealed record ReadMessagesRequest(long UserId, long ChatId, long LastReadMessageId);