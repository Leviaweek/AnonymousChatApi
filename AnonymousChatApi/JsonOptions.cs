using System.Text.Json;

namespace AnonymousChatApi;

public static class JsonOptions
{
    public static JsonSerializerOptions Options { get; } = new()
    {
        Converters =
        {
            new Cysharp.Serialization.Json.UlidJsonConverter()
        },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}