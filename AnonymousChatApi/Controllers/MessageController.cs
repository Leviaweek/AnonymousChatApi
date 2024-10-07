using System.Security.Claims;
using AnonymousChatApi.Databases;
using AnonymousChatApi.Jwt;
using AnonymousChatApi.Models;
using AnonymousChatApi.Models.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AnonymousChatApi.Controllers;

[Route("/message")]
public sealed class MessageController(AnonymousChatDb anonymousChatDb, Jwt<JwtPayload> jwt): ControllerBase
{
    [HttpGet("getMessages")]
    public Results<Ok<List<ChatMessage>>, NotFound> GetMessages([FromBody]GetMessagesRequest request)
    {
        var token = User.FindFirstValue(Constants.JwtTokenClaimType);
        var userId = User.FindFirstValue(Constants.JwtUserIdClaimType);

        if (token is null || userId is null)
            return TypedResults.NotFound();

        var ulidId = Ulid.Parse(userId);
        
        return TypedResults.Ok(anonymousChatDb.GetMessages(request.ChatId, ulidId));
    }

    [HttpPost]
    public async Task<Results<Ok<ChatMessageDto>, BadRequest>> SendMessageAsync([FromBody]ChatMessage message, CancellationToken cancellationToken)
    {
        var token = User.FindFirstValue(Constants.JwtTokenClaimType);
        var userId = User.FindFirstValue(Constants.JwtUserIdClaimType);

        if (token is null || userId is null)
            return TypedResults.BadRequest();

        var ulidId = Ulid.Parse(userId);
        
        var addedMessage = await anonymousChatDb.AddMessageAsync(ulidId, message, cancellationToken);
        if (addedMessage is null)
            return TypedResults.BadRequest();
        
        return TypedResults.Ok(addedMessage.ToDto());
    }
}