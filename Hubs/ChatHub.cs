using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WebChat.Data;
using WebChat.Models;

namespace WebChat;

[Authorize]
public class ChatHub(AppDbContext db, ILogger<ChatHub> logger) : Hub
{
    private static readonly Dictionary<string, string> _connectedUsers = [];
    private readonly AppDbContext _db = db;
    private readonly ILogger<ChatHub> _logger = logger;

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation("User {UserId} connected", Context.UserIdentifier);
        _connectedUsers.Add(Context.UserIdentifier!, Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("User {UserId} disconnected", Context.UserIdentifier);
        _connectedUsers.Remove(Context.UserIdentifier!);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(Message message)
    {
        _logger.LogInformation("User {UserId} sent message to {ReceiverId} with text {MessageText}", Context.UserIdentifier, message.ReceiverId, message.Text);

        message.SenderId = Context.UserIdentifier!;
        message.CreatedAt = DateTime.UtcNow;
        message.IsRead = false;

        if (_connectedUsers.ContainsKey(message.ReceiverId))
        {
            await Clients.Client(_connectedUsers[message.ReceiverId]).SendAsync("ReceiveMessage", message);
        }

        await _db.Messages.AddAsync(message);
        await _db.SaveChangesAsync();
    }
}