using System.ComponentModel.DataAnnotations;

namespace MiniMessenger.Api.DTOs;

public class SendMessageDto
{
    [Required]
    public string ReceiverId { get; set; } = string.Empty;

    [Required, MaxLength(2000)]
    public string Content { get; set; } = string.Empty;
}