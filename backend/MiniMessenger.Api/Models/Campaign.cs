// namespace MiniMessenger.Api.Models;

// public class Campaign
// {
//     public int Id { get; set; }

//     public string Name { get; set; } = string.Empty;
//     public MessageChannel Channel { get; set; }
//     public string? Subject { get; set; }                 // utilisé pour les e-mails
//     public string Template { get; set; } = string.Empty; // ex. "Bonjour {{prenom}}, ..."

//     public CampaignStatus Status { get; set; } = CampaignStatus.Draft;

//     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
//     public DateTime? ScheduledAt { get; set; }
//     public DateTime? StartedAt { get; set; }
//     public DateTime? CompletedAt { get; set; }

//     public int OwnerId { get; set; }
//     public User Owner { get; set; } = null!;

//     public List<Delivery> Deliveries { get; set; } = new();
// }




using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MiniMessenger.Api.Models;

public class Campaign
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public MessageChannel Channel { get; set; }
    public string? Subject { get; set; }                 // utilisé pour les e-mails
    public string Template { get; set; } = string.Empty; // ex. "Bonjour {{prenom}}, ..."

    public CampaignStatus Status { get; set; } = CampaignStatus.Draft;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string OwnerId { get; set; } = string.Empty;
}