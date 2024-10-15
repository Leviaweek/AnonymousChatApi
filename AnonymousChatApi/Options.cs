using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnonymousChatApi;

public static class Options
{
    public static JsonSerializerOptions JsonOptions { get; } = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static CookieOptions Cookie(TimeSpan lifetime, bool isHttps = false) =>
        new()
        {
            Secure = isHttps,
            HttpOnly = true,
            Expires = DateTimeOffset.UtcNow.Add(lifetime),
            SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax
        };
}