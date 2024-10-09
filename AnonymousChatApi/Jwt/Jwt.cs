using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AnonymousChatApi.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace AnonymousChatApi.Jwt;

public sealed class Jwt<T>: IDisposable where T: IJwtPayload
{
    private readonly HMACSHA256 _hmacsha256;
    public Jwt(string secret)
    {
        var bytes = Encoding.UTF8.GetBytes(secret);
        _hmacsha256 = new HMACSHA256(bytes);
    }
    
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
        var headerPayloadBytes = Encoding.UTF8.GetBytes(headerPayload);
        var signatureBytes = _hmacsha256.ComputeHash(headerPayloadBytes);
        return Base64UrlEncoder.Encode(signatureBytes);
    }

    public bool IsValid(string token)
    {
        ReadOnlySpan<char> tokenSpan = token;
        Span<Range> destination = stackalloc Range[4];
        var read =  tokenSpan.Split(destination, '.');
        if (read != 3)
            return false;
        
        var encodedHeader = token[destination[0]];
        var encodedPayload = token[destination[1]];
        var providedSignature = token[destination[2]];

        var payload = T.FromToken(token);

        if (payload.CreatedAt.Add(payload.LifeTime) < DateTimeOffset.UtcNow)
        {
            return false;
        }
        
        var headerPayload = $"{encodedHeader}.{encodedPayload}";
        
        var calculatedSignature = CreateSignature(headerPayload);
        
        if (providedSignature != calculatedSignature)
            return false;

        return true;
    }

    public void Dispose()
    {
        _hmacsha256.Dispose();
        GC.SuppressFinalize(this);
    }

    ~Jwt()
    {
        Dispose();
    }
}