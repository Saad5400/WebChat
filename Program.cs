using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebCha.Data;
using WebChat.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
builder.Services.AddAuthorization();

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();

var app = builder.Build();

app.MapIdentityApi<User>();

var api = app.MapGroup("/api");
var messages = api.MapGroup("/messages").RequireAuthorization();

messages.MapGet("/", async (AppDbContext db, ClaimsPrincipal user) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    var messages = await db.Messages
        .Where(m => m.ReceiverId == userId)
        .OrderByDescending(m => m.CreatedAt)
        .GroupBy(m => m.ReceiverId)
        .ToListAsync();

    return messages;
});

messages.MapGet("/{id}", async (AppDbContext db, string id, ClaimsPrincipal user) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    var messages = await db.Messages
        .Where(m => m.ReceiverId == userId && m.SenderId == id)
        .OrderByDescending(m => m.CreatedAt)
        .ToListAsync();

    return messages;
});

messages.MapPost("/", async (AppDbContext db, Message message, ClaimsPrincipal user) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    message.SenderId = userId!;
    message.ReceiverId = message.ReceiverId;
    await db.Messages.AddAsync(message);
    await db.SaveChangesAsync();

    return Results.Created($"/api/messages/{message.Id}", message);
});

app.Run();
