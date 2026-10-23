namespace MiniMessenger.Api.Models;

public class Delivery
{
    public long Id { get; set; }   // long : cette table peut devenir très grosse

    public int CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;

    public int ContactId { get; set; }
    public Contact Contact { get; set; } = null!;

    public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;

    public int Attempts { get; set; }              // nombre de tentatives d'envoi
    public string? LastError { get; set; }         // dernière erreur rencontrée
    public DateTime? NextAttemptAt { get; set; }   // prochaine tentative (backoff)

    public string? ProviderMessageId { get; set; } // identifiant côté fournisseur

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SentAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}