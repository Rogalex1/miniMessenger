// namespace MiniMessenger.Api.Models;

// public class Message
// {
//     public int Id { get; set; }
//     public string Content { get; set; } = string.Empty;
//     public DateTime SentAt { get; set; } = DateTime.UtcNow;

//     public int SenderId { get; set; }
//     public User Sender { get; set; } = null!;

//     public int ReceiverId { get; set; }
//     public User Receiver { get; set; } = null!;
// }



// (On remplace les entités de navigation par les identifiants 
// textuels SenderId et ReceiverId).

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MiniMessenger.Api.Models;

public class Message
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    [BsonRepresentation(BsonType.ObjectId)]
    public string SenderId { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string ReceiverId { get; set; } = string.Empty;
}