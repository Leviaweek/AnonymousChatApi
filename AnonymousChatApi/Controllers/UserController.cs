using AnonymousChatApi.Databases;
using AnonymousChatApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AnonymousChatApi.Controllers;

[Route("/user")]
public sealed class UserController(AnonymousChatDb db): ControllerBase
{
    [HttpPost("login")]
    public Results<Ok<User>, NotFound> LoginUser([FromQuery] string login, [FromQuery] string password)
    {
        var user = db.GetUser(login, password);
        
        if (user is null)
            return TypedResults.NotFound();
        
        return TypedResults.Ok(user);
    }

    [HttpPost("register")]
    public Ok<User> RegisterUser([FromQuery] string login, [FromQuery] string password)
    {
        var user = db.AddUser(login, password);
        return TypedResults.Ok(user);
    }
}