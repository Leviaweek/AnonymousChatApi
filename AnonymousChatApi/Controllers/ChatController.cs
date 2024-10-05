using AnonymousChatApi.Databases;
using AnonymousChatApi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AnonymousChatApi.Controllers;

[Route("/chat")]
public sealed class ChatController(AnonymousChatDb db): ControllerBase
{
    [HttpPost("create")]
    public Ok<Chat> CreateChat([FromQuery] string name)
    {
        var chat = db.AddChat(name);
        return TypedResults.Ok(chat);
    }

    [HttpPost("join")]
    public Results<Ok, BadRequest> Join([FromQuery] Ulid userId, [FromQuery] Ulid chatId)
    {
        var result = db.AddUserToChat(userId, chatId);
        if (!result)
            return TypedResults.BadRequest();

        return TypedResults.Ok();
    }

    [HttpPost("join-random")]
    public Results<Ok<Chat>, NotFound> JoinRandom([FromQuery] Ulid userId)
    {
        var chat = db.GetRandomChat();

        var result = db.AddUserToChat(userId, chat.Id);

        if (!result)
            return TypedResults.NotFound();
        
        return TypedResults.Ok(chat);
    }
}