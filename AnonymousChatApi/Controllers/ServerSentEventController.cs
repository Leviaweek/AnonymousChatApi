using System.Security.Claims;
using AnonymousChatApi.Databases;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using EventHandler = AnonymousChatApi.Services.EventHandler;

namespace AnonymousChatApi.Controllers;

[ApiController]
[Route("event")]
public sealed class ServerSentEventController(
    AnonymousChatDb db,
    EventHandler handler) : ControllerBase
{
    [HttpGet("subscribe")]
    public async Task<Results<EmptyHttpResult, NotFound>> SubscribeAsync(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(Constants.JwtUserIdClaimType);

        if (userId is null)
            return TypedResults.NotFound();

        var ulidId = Ulid.Parse(userId);
        
        var user = db.GetUserById(ulidId);
        
        if (user is null)
            return TypedResults.NotFound();
        
        HttpContext.Response.Headers.Append("Content-Type", "text/event-stream");
        HttpContext.Response.Headers.Append("Cache-Control", "no-cache");
        HttpContext.Response.Headers.Append("X-Accel-Buffering", "no");
        var task = handler.SubscribeOnEventAsync(ulidId, EventActionAsync, cancellationToken);

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