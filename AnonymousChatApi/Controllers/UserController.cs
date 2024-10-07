using AnonymousChatApi.Databases;
using AnonymousChatApi.Jwt;
using AnonymousChatApi.Models;
using AnonymousChatApi.Models.Requests;
using AnonymousChatApi.Models.Responses;
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

        var token = GetToken(user.Id);
        
        HttpContext.Response.Cookies.Append(Constants.CookieTokenString, token);
        
        return TypedResults.Ok();
    }

    [HttpPost("register")]
    public Results<Ok, BadRequest> RegisterUser([FromBody] RegisterUserRequest request)
    {
        var (login, password) = request;
        if (db.ContainsUser(login))
            return TypedResults.BadRequest();
            
        var user = db.AddUser(login, password);

        var token = GetToken(user.Id);
        
        HttpContext.Response.Cookies.Append(Constants.CookieTokenString, token);

        return TypedResults.Ok();
    }
    
    private string GetToken(Ulid id)
    {
        var payload = new JwtPayload(id);
        var token = jwt.CreateToken(payload);
        return token;
    }
}