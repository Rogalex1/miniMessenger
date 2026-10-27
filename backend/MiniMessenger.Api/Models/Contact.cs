// namespace MiniMessenger.Api.Models;

// public class Contact
// {
//     public int Id { get; set; }

//     public string FirstName { get; set; } = string.Empty;
//     public string? LastName { get; set; }

//     public string? PhoneNumber { get; set; }   // format international E.164, ex. +22997000000
//     public string? Email { get; set; }

//     public bool IsOptedOut { get; set; }       // désinscrit : ne reçoit plus aucun message
//     public DateTime? OptedOutAt { get; set; }

//     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

//     public int OwnerId { get; set; }
//     public User Owner { get; set; } = null!;
// }


using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MiniMessenger.Api.Models;

public class Contact
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }   // format international E.164, ex. +22997000000
    public string? Email { get; set; }

    public bool IsOptedOut { get; set; }       // désinscrit : ne reçoit plus aucun message
    public DateTime? OptedOutAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonRepresentation(BsonType.ObjectId)]
    public string OwnerId { get; set; } = string.Empty;
}