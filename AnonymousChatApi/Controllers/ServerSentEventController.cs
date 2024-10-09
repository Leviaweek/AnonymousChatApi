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
        var userIdString = User.FindFirstValue(Constants.JwtUserIdClaimType);
        var lifeTimeString = User.FindFirstValue(Constants.JwtLifeTimeClaimType);
        var createdAtString = User.FindFirstValue(Constants.JwtCreatedAtClaimType);

        if (userIdString is null || lifeTimeString is null || createdAtString is null)
            return TypedResults.NotFound();

        var ulidId = Ulid.Parse(userIdString);
        var lifeTime = TimeSpan.Parse(lifeTimeString);
        var createdAt = DateTimeOffset.Parse(createdAtString);
        
        var user = db.GetUserById(ulidId);
        
        if (user is null)
            return TypedResults.NotFound();
        
        HttpContext.Response.Headers.Append("Content-Type", "text/event-stream");
        HttpContext.Response.Headers.Append("Cache-Control", "no-cache");
        HttpContext.Response.Headers.Append("X-Accel-Buffering", "no");

        var delay = createdAt.Add(lifeTime) - DateTimeOffset.UtcNow;

        if (delay < TimeSpan.Zero)
        {
            Console.WriteLine("Dropped");
            return TypedResults.NotFound();
        }
        
        using var source = new CancellationTokenSource(delay);
        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(source.Token, cancellationToken);

        try
        {
            var task = handler.SubscribeOnEventAsync(ulidId, EventActionAsync, linkedSource.Token);

            await task;
        }
        catch (OperationCanceledException)
        {
        }

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