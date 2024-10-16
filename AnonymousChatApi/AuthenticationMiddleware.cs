using System.Security.Claims;
using System.Text.Json;
using AnonymousChatApi.Jwt;
using AnonymousChatApi.Models;

namespace AnonymousChatApi;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Jwt<JwtPayload> _jwt;

    public AuthenticationMiddleware(RequestDelegate next, Jwt<JwtPayload> jwt)
    {
        _next = next;
        _jwt = jwt;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            if (context.Request.Cookies.TryGetValue(Constants.CookieAccessTokenString, out var token))
            {
                if (_jwt.IsValid(token))
                {
                    var payload = JwtPayload.FromToken(token);

                    List<Claim> claims =
                    [
                        new(Constants.JwtUserIdClaimType, payload.UserId.ToString()),
                        new(Constants.JwtLifeTimeClaimType, payload.LifeTime.ToString()),
                        new(Constants.JwtCreatedAtClaimType, payload.CreatedAt.ToString()),
                    ];

                    var identity = new ClaimsIdentity(claims);

                    context.User.AddIdentity(identity);
                }

            }

            if (context.Request.Cookies.TryGetValue(Constants.CookieRefreshTokenString, out token))
            {
                if (_jwt.IsValid(token))
                {
                    Console.WriteLine("refresh valid");
                    var payload = JwtPayload.FromToken(token);

                    List<Claim> claims =
                    [
                        new(Constants.JwtUserIdClaimType, payload.UserId.ToString()),
                        new(Constants.JwtHasRefreshTokenType, true.ToString())
                    ];

                    var identity = new ClaimsIdentity(claims);

                    context.User.AddIdentity(identity);
                }
            }
        }
        catch (JsonException)
        { }

        await _next(context);
    }
}