using System.Security.Claims;
using AnonymousChatApi.Jwt;
using AnonymousChatApi.Models;

namespace AnonymousChatApi;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _jwtSecret;

    public AuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        var jwtSecret = configuration[JwtConfigOptions.OptionsName];
        _jwtSecret = jwtSecret ?? throw new ArgumentException("Invalid jwt secret");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Cookies.TryGetValue(Constants.CookieTokenString, out var token))
        {
            var jwt = new Jwt<JwtPayload>(_jwtSecret);

            if (jwt.IsValid(token))
            {
                var payload = JwtPayload.FromToken(token);
                
                List<Claim> claims = [new("token", token), new("userId", payload.Id.ToString())];
        
                var identity = new ClaimsIdentity(claims);
                
                context.User.AddIdentity(identity);
            }
        }
        await _next(context);
    }
}