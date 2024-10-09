using System.Security.Claims;
using AnonymousChatApi.Databases;
using AnonymousChatApi.Jwt;
using AnonymousChatApi.Models;
using AnonymousChatApi.Models.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AnonymousChatApi.Controllers;

[Route("/user")]
public sealed class UserController(AnonymousChatDb db, Jwt<JwtPayload> jwt): ControllerBase
{
    [HttpPost("login")]
    public Results<Ok, NotFound> LoginUser([FromBody] LoginUserRequest request)
    {
        var (login, password) = request;
        var user = db.GetUser(login, password);
        
        if (user is null)
            return TypedResults.NotFound();

        var accessToken = GetAccessToken(user.Id);
        var refreshToken = GetRefreshToken(user.Id);
        
        HttpContext.Response.Cookies.Append(Constants.CookieAccessTokenString, accessToken, Options.CookieOptions);
        HttpContext.Response.Cookies.Append(Constants.CookieRefreshTokenString, refreshToken, Options.CookieOptions);
        
        return TypedResults.Ok();
    }

    [HttpPost("register")]
    public Results<Ok, BadRequest> RegisterUser([FromBody] RegisterUserRequest request)
    {
        var (login, password) = request;
        if (db.ContainsUser(login))
            return TypedResults.BadRequest();
            
        var user = db.AddUser(login, password);

        var accessToken = GetAccessToken(user.Id);
        var refreshToken = GetRefreshToken(user.Id);
        
        HttpContext.Response.Cookies.Append(Constants.CookieAccessTokenString, accessToken, Options.CookieOptions);
        HttpContext.Response.Cookies.Append(Constants.CookieRefreshTokenString, refreshToken, Options.CookieOptions);

        return TypedResults.Ok();
    }

    [HttpPost("refresh-access-token")]
    public Results<Ok, BadRequest> RefreshAccessToken()
    {
        var hasRefreshTokenValue = User.FindFirstValue(Constants.JwtHasRefreshTokenType);
        var userId = User.FindFirstValue(Constants.JwtUserIdClaimType);
        
        if (hasRefreshTokenValue is null || userId is null)
            return TypedResults.BadRequest();

        var hasRefreshToken = bool.Parse(hasRefreshTokenValue);

        if (!hasRefreshToken)
            return TypedResults.BadRequest();

        var userIdUlid = Ulid.Parse(userId);

        var user = db.GetUserById(userIdUlid);

        if (user is null)
            return TypedResults.BadRequest();

        var accessToken = GetAccessToken(userIdUlid);
        
        HttpContext.Response.Cookies.Append(Constants.CookieAccessTokenString, accessToken, Options.CookieOptions);
        return TypedResults.Ok();
    }
    
    private string GetAccessToken(Ulid id)
    {
        var payload = new JwtPayload(id, DateTimeOffset.UtcNow, Constants.AccessTokenLifeTime);
        var token = jwt.CreateToken(payload);
        return token;
    }

    private string GetRefreshToken(Ulid id)
    {
        var payload = new JwtPayload(id, DateTimeOffset.UtcNow, Constants.RefreshTokenLifeTime, true);
        var token = jwt.CreateToken(payload);
        return token;
    }
}