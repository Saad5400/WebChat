using System.ComponentModel.DataAnnotations;

namespace WebChat.Models;

public class Message
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Text { get; set; } = null!;

    public string SenderId { get; set; } = string.Empty;

    [Required]
    public string ReceiverId { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;

    public virtual User Sender { get; set; } = null!;
    public virtual User Receiver { get; set; } = null!;
}
