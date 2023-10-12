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

app.UseStaticFiles();

var api = app.MapGroup("/api");

api.MapIdentityApi<User>();

var users = api.MapGroup("/users");

users.MapGet("/", async (AppDbContext db) =>
{
    var users = await db.Users.ToListAsync();
    var dto = users.Select(u => new
    {
        u.Id,
        u.Email,
    });
    return dto;
}).RequireAuthorization();

var messages = api.MapGroup("/messages").RequireAuthorization();

messages.MapGet("/", async (AppDbContext db, ClaimsPrincipal user) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    var messages = await db.Messages
        .Where(m => m.SenderId == userId || m.ReceiverId == userId)
        .OrderByDescending(m => m.CreatedAt)
        .GroupBy(m => m.ReceiverId)
        .ToListAsync();

    return messages;
});

messages.MapGet("/{id}", async (AppDbContext db, string id, ClaimsPrincipal user) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    var messages = await db.Messages
        .Where(
            m =>
                (m.ReceiverId == userId && m.SenderId == id) ||
                (m.ReceiverId == id && m.SenderId == userId)
        )
        .OrderByDescending(m => m.CreatedAt)
        .ToListAsync();

    return messages;
});

messages.MapPost("/", async (AppDbContext db, Message message, ClaimsPrincipal user) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    message.SenderId = userId!;
    if (message.ReceiverId == message.SenderId)
    {
        return Results.BadRequest("You can't send message to yourself");
    }
    if (!await db.Users.AnyAsync(u => u.Id == message.ReceiverId))
    {
        return Results.BadRequest("Receiver not found");
    }
    await db.Messages.AddAsync(message);
    await db.SaveChangesAsync();

    return Results.Created($"/api/messages/{message.Id}", message);
});

// return index.html
// app.MapGet("/", () => Results.Content(File.ReadAllText("wwwroot/index.html"), "text/html"));
app.MapFallbackToFile("index.html");

app.Run();
