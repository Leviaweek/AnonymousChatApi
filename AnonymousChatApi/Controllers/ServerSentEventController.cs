using System.Text.Json;
using AnonymousChatApi.Abstractions;
using AnonymousChatApi.Databases;
using AnonymousChatApi.Models;
using AnonymousChatApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AnonymousChatApi.Controllers;

[ApiController]
[Route("event")]
public sealed class ServerSentEventController(
    AnonymousChatDb db,
    EventMessageHandler messageHandler) : ControllerBase
{
    [HttpGet("subscribe")]
    public async Task<Results<EmptyHttpResult, NotFound>> SubscribeAsync([FromQuery] string userId,
        CancellationToken cancellationToken)
    {
        if (!Ulid.TryParse(userId, out var userIdUlid))
            return TypedResults.NotFound();

        var user = db.GetUserById(userIdUlid);
        if (user is null)
            return TypedResults.NotFound();
        
        HttpContext.Response.Headers.Append("Content-Type", "text/event-stream");
        HttpContext.Response.Headers.Append("Cache-Control", "no-cache");
        HttpContext.Response.Headers.Append("X-Accel-Buffering", "no");
        var task = messageHandler.SubscribeOnMessageAsync(userIdUlid, EventActionAsync, cancellationToken);

        await task;

        return TypedResults.Empty;
    }

    private async Task EventActionAsync(string eventName, string body, CancellationToken cancellationToken)
    {
        await HttpContext.Response.WriteAsync($"event: {eventName}\n", cancellationToken);
        await HttpContext.Response.WriteAsync("data: ", cancellationToken);
        await HttpContext.Response.WriteAsync(body, cancellationToken: cancellationToken);
        await HttpContext.Response.WriteAsync("\n\n", cancellationToken);
        await HttpContext.Response.Body.FlushAsync(cancellationToken);
    }
}