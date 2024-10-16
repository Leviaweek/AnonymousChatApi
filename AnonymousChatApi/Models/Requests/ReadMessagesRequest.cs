namespace AnonymousChatApi.Models.Requests;

public sealed record ReadMessagesRequest(long UserId, long ChatId, long LastReadMessageId);