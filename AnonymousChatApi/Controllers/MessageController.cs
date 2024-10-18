using System.Security.Claims;
using AnonymousChatApi.Databases;
using AnonymousChatApi.Models.Dtos;
using AnonymousChatApi.Models.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AnonymousChatApi.Controllers;

[Route("/message")]
public sealed class MessageController(AnonymousChatDb anonymousChatDb): ControllerBase
{
    [HttpGet("get-messages")]
    public async Task<Results<Ok<List<MessageDto>>, NotFound>> GetMessagesAsync(
        [FromQuery] long chatId,
        [FromQuery] string sortingType,
        CancellationToken cancellationToken,
        [FromQuery] long countFrom = long.MaxValue,
        [FromQuery] int count = 50)
    {
        var userId = User.FindFirstValue(Constants.JwtUserIdClaimType);
        var lifeTime = User.FindFirstValue(Constants.JwtLifeTimeClaimType);

        if (userId is null || lifeTime is null)
            return TypedResults.NotFound();

        bool isAsc;

        switch (sortingType)
        {
            case "asc":
                isAsc = true;
                break;
            case "desc":
                isAsc = false;
                break;
            default:
                return TypedResults.NotFound();
        }
        
        var userIdLong = long.Parse(userId);
        
        var messages = await anonymousChatDb.GetMessages(chatId,
            userIdLong,
            countFrom,
            isAsc,
            count,
            cancellationToken);
        
        if (messages is null)
            return TypedResults.NotFound();
        
        return TypedResults.Ok(messages);
    }

    [HttpPost]
    public async Task<Results<Ok<MessageDto>, BadRequest>> SendMessageAsync([FromBody] MessageDto message,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(Constants.JwtUserIdClaimType);
        var lifeTime = User.FindFirstValue(Constants.JwtLifeTimeClaimType);

        if (userId is null || lifeTime is null)
            return TypedResults.BadRequest();

        var userIdLong = long.Parse(userId);
        
        if (message.SenderId != userIdLong)
            return TypedResults.BadRequest();
        
        var resultMessage = await anonymousChatDb.AddTextMessageAsync(message, cancellationToken);
        
        if (resultMessage is null)
            return TypedResults.BadRequest();
        
        return TypedResults.Ok(resultMessage);
    }

    [HttpPost("read")]
    public async Task<Results<Ok, BadRequest>> ReadMessagesAsync([FromBody] ReadMessagesRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(Constants.JwtUserIdClaimType);
        var lifeTime = User.FindFirstValue(Constants.JwtLifeTimeClaimType);

        if (userId is null || lifeTime is null)
            return TypedResults.BadRequest();

        var userIdLong = long.Parse(userId);
        
        if (request.UserId != userIdLong)
        {
            Console.WriteLine(request);
            return TypedResults.BadRequest();
        }

        var result = await anonymousChatDb.TryReadMessagesAsync(request.UserId, request.ChatId,
            request.LastReadMessageId, cancellationToken);

        if (!result)
            return TypedResults.BadRequest();

        return TypedResults.Ok();
    }

    [HttpGet("updates")]
    public async Task<Results<Ok<int>, NotFound>> GetUpdatesCountAsync([FromQuery] long chatId,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(Constants.JwtUserIdClaimType);
        var lifeTime = User.FindFirstValue(Constants.JwtLifeTimeClaimType);

        if (userId is null || lifeTime is null)
            return TypedResults.NotFound();

        var result = await anonymousChatDb.GetUpdatesCountAsync(chatId, cancellationToken);

        if (result is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(result.Value);
    }
}