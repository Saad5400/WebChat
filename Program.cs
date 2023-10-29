using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebChat;
using WebChat.Data;
using WebChat.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
});

// Auth
builder.Services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme, options =>
    {
        // options.BearerTokenExpiration = TimeSpan.FromSeconds(10);
        // options.RefreshTokenExpiration = TimeSpan.FromSeconds(60);
        options.Events = new BearerTokenEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/api/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            },
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 0;
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});

// OutputCache
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromDays(1)));
});

// SignalR
builder.Services.AddSignalR();

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions()
{
    HttpsCompression = Microsoft.AspNetCore.Http.Features.HttpsCompressionMode.Compress,
    OnPrepareResponse = ctx =>
    {
        // Cache
        // ctx.Context.Response.Headers.Append("Cache-Control", $"public,max-age={1 * 60 * 60 * 24}");
        // ctx.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddDays(1).ToString("R", CultureInfo.InvariantCulture));
    }
});
app.UseCors();
// app.UseOutputCache();
app.UseAuthentication();
app.UseAuthorization();

var api = app.MapGroup("/api");
var auth = api.MapGroup("/auth");
var hubs = api.MapGroup("/hubs");
var users = api.MapGroup("/users").RequireAuthorization();
var messages = api.MapGroup("/messages").RequireAuthorization();
var chats = api.MapGroup("/chats").RequireAuthorization();

auth.MapIdentityApi<User>();

hubs.MapHub<ChatHub>("/chat");

users.MapGet("/", async (AppDbContext db, [FromQuery] string? email, [FromQuery] string? id) =>
{
    email ??= "";
    id ??= "";

    if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(id))
    {
        return Results.BadRequest("Email or Id is required");
    }

    var users = await db.Users
        .Where(u => u.Email == email || u.Id == id)
        .Select(u => new
        {
            u.Id,
            u.Email,
        })
        .ToListAsync();

    return Results.Ok(users);
});

users.MapPost("/search", async (AppDbContext db, [FromBody] string q) =>
{
    if (string.IsNullOrWhiteSpace(q) || q.Length < 5)
    {
        return Results.BadRequest("Query must be at least 5 characters long");
    }
    var users = await db.Users.Where(u => u.Email!.Equals(q)).ToListAsync();
    var res = users.Select(u => new
    {
        u.Id,
        u.Email,
    });
    return Results.Ok(res);
});

chats.MapGet("/", async (AppDbContext db, ClaimsPrincipal user) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    var chats = await db.Messages
        .Where(m => m.ReceiverId == userId || m.SenderId == userId)
        .GroupBy(m => m.ReceiverId == userId ? m.SenderId : m.ReceiverId)
        .Select(g => new
        {
            User = db.Users.Select(u => new
            {
                u.Id,
                u.Email,
            }).FirstOrDefault(u => u.Id == g.Key),
            LastMessage = g.Select(m => new
            {
                m.Id,
                m.Text,
                m.CreatedAt,
                m.IsRead,
                m.SenderId,
                m.ReceiverId,
            }).OrderByDescending(m => m.CreatedAt).FirstOrDefault(),
        })
        .ToListAsync();
    return chats;
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
        .OrderBy(m => m.CreatedAt)
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
app.MapFallbackToFile("index.html").CacheOutput();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
