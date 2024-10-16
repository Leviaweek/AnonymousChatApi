using System.Security.Claims;
using AnonymousChatApi.Databases;
using AnonymousChatApi.Jwt;
using AnonymousChatApi.Models;
using AnonymousChatApi.Models.Dtos;
using AnonymousChatApi.Models.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AnonymousChatApi.Controllers;

[Route("/user")]
public sealed class UserController(AnonymousChatDb db, Jwt<JwtPayload> jwt): ControllerBase
{
    [HttpPost("login")]
    public async Task<Results<Ok<UserDto>, NotFound>> LoginUserAsync([FromBody] LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var (login, password) = request;
        var user = await db.GetUserAsync(login, password, cancellationToken);
        
        if (user is null)
            return TypedResults.NotFound();

        var accessToken = GetAccessToken(user.Id);
        var refreshToken = GetRefreshToken(user.Id);
        
        HttpContext.Response.Cookies.Append(Constants.CookieAccessTokenString, accessToken,
            Options.Cookie(Constants.AccessTokenLifeTime, HttpContext.Request.IsHttps));
        HttpContext.Response.Cookies.Append(Constants.CookieRefreshTokenString, refreshToken,
            Options.Cookie(Constants.RefreshTokenLifeTime, HttpContext.Request.IsHttps));
        
        return TypedResults.Ok(user);
    }

    [HttpPost("register")]
    public async Task<Results<Ok<UserDto>, BadRequest>> RegisterUserAsync([FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        var (login, password) = request;
        if (login.Length < 6 || password.Length < 6)
		{
            return TypedResults.BadRequest();
		}
            
        var user = await db.AddUserAsync(login, password, cancellationToken);

        if (user is null)
            return TypedResults.BadRequest();
        
        var accessToken = GetAccessToken(user.Id);
        var refreshToken = GetRefreshToken(user.Id);
        
        HttpContext.Response.Cookies.Append(Constants.CookieAccessTokenString, accessToken,
            Options.Cookie(Constants.AccessTokenLifeTime, HttpContext.Request.IsHttps));
        HttpContext.Response.Cookies.Append(Constants.CookieRefreshTokenString,
            refreshToken,
            Options.Cookie(Constants.RefreshTokenLifeTime, HttpContext.Request.IsHttps));


        return TypedResults.Ok(user);
    }

    [HttpPost("refresh-access-token")]
    public async Task<Results<Ok, BadRequest>> RefreshAccessTokenAsync(CancellationToken cancellationToken)
    {
        var hasRefreshTokenValue = User.FindFirstValue(Constants.JwtHasRefreshTokenType);
        var userId = User.FindFirstValue(Constants.JwtUserIdClaimType);
        
        if (hasRefreshTokenValue is null || userId is null)
        {
            Console.WriteLine(hasRefreshTokenValue is null);
            return TypedResults.BadRequest();
        }

        var hasRefreshToken = bool.Parse(hasRefreshTokenValue);

        if (!hasRefreshToken)
        {
            Console.WriteLine("not refesh token");
            return TypedResults.BadRequest();
        }

        var userIdLong = long.Parse(userId);

        var user = await db.GetUserByIdAsync(userIdLong, cancellationToken);

        if (user is null)
        {
            Console.WriteLine("Not user in db");
            return TypedResults.BadRequest();
        }

        var accessToken = GetAccessToken(userIdLong);
        
        HttpContext.Response.Cookies.Append(Constants.CookieAccessTokenString, accessToken,
            Options.Cookie(Constants.AccessTokenLifeTime, HttpContext.Request.IsHttps));
        return TypedResults.Ok();
    }
    
    private string GetAccessToken(long id)
    {
        var payload = new JwtPayload(id, DateTimeOffset.UtcNow, Constants.AccessTokenLifeTime);
        var token = jwt.CreateToken(payload);
        return token;
    }

    private string GetRefreshToken(long id)
    {
        var payload = new JwtPayload(id, DateTimeOffset.UtcNow, Constants.RefreshTokenLifeTime, true);
        var token = jwt.CreateToken(payload);
        return token;
    }
}