// namespace MiniMessenger.Api.Models;

// public class User
// {
//     public int Id { get; set; }
//     public string Username { get; set; } = string.Empty;
//     public string PasswordHash { get; set; } = string.Empty;
//     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

//     public List<Message> SentMessages { get; set; } = new();
//     public List<Message> ReceivedMessages { get; set; } = new();
// }




/////(En NoSQL, on évite d'imbriquer des listes géantes de messages directement dans l'utilisateur pour ne pas
/// alourdir le document. On garde juste ses infos principales).

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MiniMessenger.Api.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}