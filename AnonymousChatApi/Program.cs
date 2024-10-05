using System.Text.Json;
using AnonymousChatApi.Databases;
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

var app = builder.Build();

app.UseRouting();
app.UseCors();
app.MapControllers();

app.Run();