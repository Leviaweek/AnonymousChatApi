using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace AnonymousChatApi.Jwt;

public class Jwt<T>(string secret)
{
    private const string Header = "{\"alg\":\"HS256\",\"typ\":\"JWT\"}";

    public string CreateToken(T payload)
    {
        var encodedHeader = Base64UrlEncoder.Encode(Header);
    
        var jsonPayload = JsonSerializer.Serialize(payload);
        var encodedPayload = Base64UrlEncoder.Encode(jsonPayload);

        var headerPayload = $"{encodedHeader}.{encodedPayload}";
    
        var signature = CreateSignature(headerPayload);

        return $"{encodedHeader}.{encodedPayload}.{signature}";
    }
    private string CreateSignature(string headerPayload)
    {
        var secretBytes = Encoding.UTF8.GetBytes(secret);
        var headerPayloadBytes = Encoding.UTF8.GetBytes(headerPayload);

        using var hmac = new HMACSHA256(secretBytes);
        var signatureBytes = hmac.ComputeHash(headerPayloadBytes);
        return Base64UrlEncoder.Encode(signatureBytes);
    }

    public bool IsValid(string token)
    {
        Span<string> parts = token.Split('.');
        if (parts.Length != 3)
            return false;

        var encodedHeader = parts[0];
        var encodedPayload = parts[1];
        var providedSignature = parts[2];

        var headerPayload = $"{encodedHeader}.{encodedPayload}";
        
        var calculatedSignature = CreateSignature(headerPayload);
        
        if (providedSignature != calculatedSignature)
            return false;
    
        return ValidateExpiration(encodedPayload);
    }

    private static bool ValidateExpiration(string encodedPayload)
    {
        var payloadJson = Base64UrlEncoder.Decode(encodedPayload);

        try
        {
            JsonSerializer.Deserialize<T>(payloadJson);
        }
        catch (JsonException)
        {
            return false;
        }

        return true;
    }
  
}