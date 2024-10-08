using System.Security.Claims;
using AnonymousChatApi.Databases;
using AnonymousChatApi.Models;
using AnonymousChatApi.Models.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AnonymousChatApi.Controllers;

[Route("/chat")]
public sealed class ChatController(AnonymousChatDb db): ControllerBase
{
    [HttpPost("create")]
    public Results<Ok<Chat>, NotFound> CreateChat([FromBody] CreateChatRequest request)
    {
        var token = User.FindFirstValue(Constants.JwtTokenClaimType);

        if (token is null)
            return TypedResults.NotFound();
        
        var chat = db.AddChat(request.Name);
        return TypedResults.Ok(chat);
    }

    [HttpPost("join")]
    public Results<Ok, BadRequest> Join([FromBody] JoinChatRequest request)
    {
        var userId = User.FindFirstValue(Constants.JwtUserIdClaimType);

        if (userId is null)
            return TypedResults.BadRequest();

        var ulidId = Ulid.Parse(userId);
        
        var result = db.AddUserToChat(ulidId, request.ChatId);
        if (!result)
            return TypedResults.BadRequest();

        return TypedResults.Ok();
    }

    [HttpPost("join-random")]
    public Results<Ok<Chat>, NotFound> JoinRandom()
    {
        var userId = User.FindFirstValue(Constants.JwtUserIdClaimType);

        if (userId is null)
            return TypedResults.NotFound();

        var ulidId = Ulid.Parse(userId);
        
        var chat = db.GetRandomChat();

        var result = db.AddUserToChat(ulidId, chat.Id);

        if (!result)
            return TypedResults.NotFound();
        
        return TypedResults.Ok(chat);
    }
}