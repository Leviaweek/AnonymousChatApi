using System.Security.Claims;
using AnonymousChatApi.Databases;
using AnonymousChatApi.Models.Dtos;
using AnonymousChatApi.Models.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AnonymousChatApi.Controllers;

[Route("/chat")]
public sealed class ChatController(AnonymousChatDb db): ControllerBase
{
    [HttpPost("create")]
    public Results<Ok<ChatDto>, NotFound> CreateChat([FromBody] CreateChatRequest request)
    {
        var token = User.FindFirstValue(Constants.JwtTokenClaimType);

        if (token is null)
            return TypedResults.NotFound();
        
        var chat = db.AddChat(request.Name);
        return TypedResults.Ok(chat.ToDto());
    }

    [HttpPost("join")]
    public async Task<Results<Ok, BadRequest>> JoinAsync([FromBody] JoinChatRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(Constants.JwtUserIdClaimType);

        if (userId is null)
            return TypedResults.BadRequest();

        var ulidId = Ulid.Parse(userId);
        
        var result = await db.AddUserToChatAsync(ulidId, request.ChatId, cancellationToken);
        
        if (!result)
            return TypedResults.BadRequest();

        return TypedResults.Ok();
    }

    [HttpPost("join-random")]
    public async Task<Results<Ok<ChatDto>, NotFound>> JoinRandomAsync(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(Constants.JwtUserIdClaimType);

        if (userId is null)
            return TypedResults.NotFound();

        var ulidId = Ulid.Parse(userId);
        
        var chat = db.GetRandomChat();

        var result = await db.AddUserToChatAsync(ulidId, chat.Id, cancellationToken);

        if (!result)
            return TypedResults.NotFound();
        
        return TypedResults.Ok(chat.ToDto());
    }
}