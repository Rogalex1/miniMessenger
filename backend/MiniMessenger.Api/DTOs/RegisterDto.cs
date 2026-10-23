using System.ComponentModel.DataAnnotations;

namespace MiniMessenger.Api.DTOs;

public class RegisterDto
{
    [Required, MinLength(3), MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
}