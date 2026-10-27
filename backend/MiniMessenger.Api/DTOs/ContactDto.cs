namespace MiniMessenger.Api.DTOs;

public class ContactDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public bool IsOptedOut { get; set; }
}