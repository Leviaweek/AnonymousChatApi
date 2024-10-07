using System.Security.Claims;
using System.Text.Json;
using AnonymousChatApi.Abstractions;
using AnonymousChatApi.Databases;
using AnonymousChatApi.Jwt;
using AnonymousChatApi.Models;
using AnonymousChatApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AnonymousChatApi.Controllers;

[ApiController]
[Route("event")]
public sealed class ServerSentEventController(
    AnonymousChatDb db,
    EventMessageHandler messageHandler,
    Jwt<JwtPayload> jwt) : ControllerBase
{
    [HttpGet("subscribe")]
    public async Task<Results<EmptyHttpResult, NotFound>> SubscribeAsync(CancellationToken cancellationToken)
    {
        var token = User.FindFirstValue(Constants.JwtTokenClaimType);
        var userId = User.FindFirstValue(Constants.JwtUserIdClaimType);

        if (token is null || userId is null)
            return TypedResults.NotFound();

        var ulidId = Ulid.Parse(userId);
        
        var user = db.GetUserById(ulidId);
        
        if (user is null)
            return TypedResults.NotFound();
        
        HttpContext.Response.Headers.Append("Content-Type", "text/event-stream");
        HttpContext.Response.Headers.Append("Cache-Control", "no-cache");
        HttpContext.Response.Headers.Append("X-Accel-Buffering", "no");
        var task = messageHandler.SubscribeOnMessageAsync(ulidId, EventActionAsync, cancellationToken);

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