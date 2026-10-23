using System.ComponentModel.DataAnnotations;

namespace MiniMessenger.Api.DTOs;

public class SendMessageDto
{
    [Required]
    public int ReceiverId { get; set; }

    [Required, MaxLength(2000)]
    public string Content { get; set; } = string.Empty;
}