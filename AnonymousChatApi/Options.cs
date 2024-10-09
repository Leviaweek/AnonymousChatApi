using System.Text.Json;
using System.Text.Json.Serialization;
using Cysharp.Serialization.Json;

namespace AnonymousChatApi;

public static class Options
{
    public static JsonSerializerOptions JsonOptions { get; } = new()
    {
        Converters =
        {
            new UlidJsonConverter()
        },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static CookieOptions CookieOptions { get; } = new()
    {
        Secure = true,
        HttpOnly = true
    };
}