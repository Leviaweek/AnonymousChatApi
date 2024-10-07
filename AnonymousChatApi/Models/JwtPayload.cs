using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace AnonymousChatApi.Models;

[Serializable]
public sealed record JwtPayload(Ulid Id)
{
    public static JwtPayload FromToken(string token)
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