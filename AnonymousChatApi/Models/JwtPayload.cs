using System.Text.Json;
using AnonymousChatApi.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace AnonymousChatApi.Models;

[Serializable]
public sealed class JwtPayload(long userId, DateTimeOffset createdAt, TimeSpan lifeTime, bool isRefreshToken = false): IJwtPayload
{

    public long UserId { get; } = userId;
    public DateTimeOffset CreatedAt { get; } = createdAt;
    public TimeSpan LifeTime { get; } = lifeTime;
    public bool IsRefreshToken { get; } = isRefreshToken;
    
    public static IJwtPayload FromToken(string token)
    {
        if (token.Split('.') is not [_, var encodedPayload, _])
        {
            throw new ArgumentException("Invalid parameter:", nameof(token));
        }

        ReadOnlySpan<char> decodedPayload = Base64UrlEncoder.Decode(encodedPayload);
        try
        {
            var payload = JsonSerializer.Deserialize<JwtPayload>(decodedPayload);
            if (payload is null)
                throw new ArgumentException("Invalid parameter:", nameof(token));
                
            return payload;
        }
        catch (JsonException)
        {
            throw new ArgumentException("Invalid parameter:", nameof(token));
        }
    }
}