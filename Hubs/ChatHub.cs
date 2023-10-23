using Microsoft.AspNetCore.SignalR;
using WebChat.Data;
using WebChat.Models;

namespace WebChat;

public class ChatHub(AppDbContext db) : Hub
{
    private static readonly Dictionary<string, string> _connectedUsers = [];
    private readonly AppDbContext _db = db;

    public override Task OnConnectedAsync()
    {
        _connectedUsers.Add(Context.UserIdentifier!, Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _connectedUsers.Remove(Context.UserIdentifier!);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string receiverId, string messageText)
    {
        if (_connectedUsers.ContainsKey(receiverId))
        {
            await Clients.Client(_connectedUsers[receiverId]).SendAsync("ReceiveMessage", messageText);
        }

        var message = new Message
        {
            SenderId = Context.UserIdentifier!,
            ReceiverId = receiverId,
            Text = messageText,
            CreatedAt = DateTime.Now,
        };
        await _db.Messages.AddAsync(message);
        await _db.SaveChangesAsync();
    }
}