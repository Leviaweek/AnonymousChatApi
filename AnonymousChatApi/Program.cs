using System.Text.Json;
using AnonymousChatApi;
using AnonymousChatApi.Databases;
using AnonymousChatApi.Jwt;
using AnonymousChatApi.Models;
using AnonymousChatApi.Services;
using Cysharp.Serialization.Json;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        corsPolicyBuilder =>
        {
            corsPolicyBuilder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddControllers();

builder.Services.AddSingleton<AnonymousChatDb>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new UlidJsonConverter());
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services.AddSingleton<EventMessageHandler>();

var jwtSecret = builder.Configuration[JwtConfigOptions.OptionsName];
if (jwtSecret is null)
    throw new ArgumentException("Invalid jwt secret");

builder.Services.AddSingleton<Jwt<JwtPayload>>(_ => new Jwt<JwtPayload>(jwtSecret));

var app = builder.Build();

app.UseMiddleware<AuthenticationMiddleware>();
app.UseRouting();
app.UseCors();
app.MapControllers();

app.Run();