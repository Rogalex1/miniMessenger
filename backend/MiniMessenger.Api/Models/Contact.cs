namespace MiniMessenger.Api.Models;

public class Contact
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }   // format international E.164, ex. +22997000000
    public string? Email { get; set; }

    public bool IsOptedOut { get; set; }       // désinscrit : ne reçoit plus aucun message
    public DateTime? OptedOutAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int OwnerId { get; set; }
    public User Owner { get; set; } = null!;
}