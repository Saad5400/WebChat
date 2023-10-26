using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebChat.Data;
using WebChat.Models;

namespace WebChat;

record ConnectionInfo(string UserId, string ConnectionId);

[Authorize]
public class ChatHub(AppDbContext db, ILogger<ChatHub> logger) : Hub
{
    private readonly AppDbContext _db = db;
    private readonly ILogger<ChatHub> _logger = logger;

    public async Task SendMessage(Message message)
    {
        // _logger.LogInformation("User {UserId} sent message to {ReceiverId} with text {MessageText}", Context.UserIdentifier, message.ReceiverId, message.Text);

        message.SenderId = Context.UserIdentifier!;
        message.CreatedAt = DateTime.UtcNow;
        message.IsRead = false;

        message.Sender = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == message.SenderId) ?? throw new InvalidOperationException("Sender not found");
        message.Receiver = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == message.ReceiverId) ?? throw new InvalidOperationException("Receiver not found");
        await Clients.Users(message.ReceiverId, message.SenderId).SendAsync("ReceiveMessage", message);

        message.Sender = null!;
        message.Receiver = null!;

        await _db.Messages.AddAsync(message);
        await _db.SaveChangesAsync();
    }
}