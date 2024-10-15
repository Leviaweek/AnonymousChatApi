namespace AnonymousChatApi.Abstractions;

public interface IJwtPayload
{
    public long UserId { get; }
    public DateTimeOffset CreatedAt { get; }
    public TimeSpan LifeTime { get; }

    public static abstract IJwtPayload FromToken(string token);
    public bool IsRefreshToken { get; }
}