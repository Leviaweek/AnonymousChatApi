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
    public Results<Ok<List<ChatMessageDto>>, NotFound> GetMessages([FromBody]GetMessagesRequest request)
    {
        var userId = User.FindFirstValue(Constants.JwtUserIdClaimType);
        var lifeTime = User.FindFirstValue(Constants.JwtLifeTimeClaimType);

        if (userId is null || lifeTime is null)
            return TypedResults.NotFound();

        var ulidId = Ulid.Parse(userId);
        var messages = anonymousChatDb.GetMessages(request.ChatId, ulidId);
        if (messages is null)
            return TypedResults.NotFound();
        return TypedResults.Ok(messages.Select(x => x.ToDto()).ToList());
    }

    [HttpPost]
    public async Task<Results<Ok<ChatMessageDto>, BadRequest>> SendMessageAsync([FromBody] ChatMessageDto message,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(Constants.JwtUserIdClaimType);
        var lifeTime = User.FindFirstValue(Constants.JwtLifeTimeClaimType);

        if (userId is null || lifeTime is null)
            return TypedResults.BadRequest();

        var ulidId = Ulid.Parse(userId);

        var dbMessage = message.ToDb(ulidId);
        
        dbMessage = await anonymousChatDb.AddMessageAsync(dbMessage, cancellationToken);
        
        if (dbMessage is null)
            return TypedResults.BadRequest();
        
        return TypedResults.Ok(dbMessage.ToDto());
    }
}