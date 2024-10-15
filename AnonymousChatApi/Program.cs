using System.Text.Json;
using System.Text.Json.Serialization;
using AnonymousChatApi;
using AnonymousChatApi.Databases;
using AnonymousChatApi.Jwt;
using AnonymousChatApi.Models;
using Microsoft.EntityFrameworkCore;
using EventHandler = AnonymousChatApi.Services.EventHandler;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        corsPolicyBuilder =>
        {
            corsPolicyBuilder
                .WithOrigins("http://localhost:5500", "http://127.0.0.1:5500")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

builder.Services.AddControllers();

builder.Services.AddDbContextFactory<AnonymousChatDbContext>(optionsBuilder => 
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<AnonymousChatDb>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddSingleton<EventHandler>();

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