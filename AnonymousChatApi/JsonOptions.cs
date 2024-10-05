using System.Text.Json;

namespace AnonymousChatApi;

public static class JsonOptions
{
    public static JsonSerializerOptions UlidOptions { get; } = new()
    {
        Converters =
        {
            new Cysharp.Serialization.Json.UlidJsonConverter()
        },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}