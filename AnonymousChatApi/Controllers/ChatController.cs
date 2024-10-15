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
    public async Task<Results<Ok<ChatDto>, NotFound>> CreateChatAsync([FromBody] CreateChatRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(Constants.JwtUserIdClaimType);
        var lifeTime = User.FindFirstValue(Constants.JwtLifeTimeClaimType);

        if (userId is null || lifeTime is null)
            return TypedResults.NotFound();

        var userIdLong = long.Parse(userId);

        if (userIdLong != request.UserId)
            return TypedResults.NotFound();
        
        var chat = await db.AddChatAsync(request.Name, cancellationToken);
        return TypedResults.Ok(chat);
    }

    [HttpPost("join")]
    public async Task<Results<Ok, BadRequest>> JoinAsync([FromBody] JoinChatRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(Constants.JwtUserIdClaimType);
        var lifeTime = User.FindFirstValue(Constants.JwtLifeTimeClaimType);

        if (userId is null || lifeTime is null)
            return TypedResults.BadRequest();

        var userIdLong = long.Parse(userId);

        if (userIdLong != request.UserId)
            return TypedResults.BadRequest();
        
        var result = await db.AddUserToChatAsync(userIdLong, request.ChatId, cancellationToken);
        
        if (!result)
            return TypedResults.BadRequest();

        return TypedResults.Ok();
    }

    [HttpPost("join-random")]
    public async Task<Results<Ok<ChatDto>, NotFound>> JoinRandomAsync(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(Constants.JwtUserIdClaimType);
        var lifeTime = User.FindFirstValue(Constants.JwtLifeTimeClaimType);

        if (userId is null || lifeTime is null)
            return TypedResults.NotFound();

        var userIdLong = long.Parse(userId);
        
        var chat = await db.GetRandomChatAsync(userIdLong, cancellationToken);

        var result = await db.AddUserToChatAsync(userIdLong, chat.Id, cancellationToken);

        if (!result)
            return TypedResults.NotFound();
        
        return TypedResults.Ok(chat);
    }
}