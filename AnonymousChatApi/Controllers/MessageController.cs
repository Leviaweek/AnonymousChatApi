using AnonymousChatApi.Databases;
using AnonymousChatApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AnonymousChatApi.Controllers;

[Route("/message")]
public sealed class MessageController(AnonymousChatDb anonymousChatDb): ControllerBase
{
    [HttpGet("getMessages")]
    public Results<Ok<List<ChatMessage>>, NotFound> GetMessages([FromQuery] Ulid userId, [FromQuery] Ulid chatId) =>
        TypedResults.Ok(anonymousChatDb.GetMessages(chatId, userId));

    [HttpPost]
    public async Task<Results<Ok<ChatMessageDto>, BadRequest>> SendMessageAsync([FromBody]ChatMessage message, CancellationToken cancellationToken)
    {
        Console.WriteLine(message);
        var addedMessage = await anonymousChatDb.AddMessageAsync(message, cancellationToken);
        if (addedMessage is null)
            return TypedResults.BadRequest();
        
        return TypedResults.Ok(addedMessage.ToDto());
    }
}