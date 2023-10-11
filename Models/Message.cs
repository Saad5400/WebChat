using System.ComponentModel.DataAnnotations;

namespace WebChat.Models;

public class Message
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string Text { get; set; } = string.Empty;

    [Required]
    public string SenderId { get; set; } = string.Empty;

    [Required]
    public string ReceiverId { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public bool IsRead { get; set; } = false;

    public virtual User Sender { get; set; } = null!;
    public virtual User Receiver { get; set; } = null!;
}
