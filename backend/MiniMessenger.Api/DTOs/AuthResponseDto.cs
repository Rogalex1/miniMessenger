namespace MiniMessenger.Api.DTOs;

public class AuthResponseDto
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}