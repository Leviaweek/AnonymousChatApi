using System.Text.Json;
using System.Text.Json.Serialization;
using Cysharp.Serialization.Json;

namespace AnonymousChatApi;

public static class JsonOptions
{
    public static JsonSerializerOptions Options { get; } = new()
    {
        Converters =
        {
            new UlidJsonConverter()
        },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}