namespace MiniMessenger.Api.Models;

public enum MessageChannel
{
    Sms = 1,
    Email = 2
}

public enum CampaignStatus
{
    Draft = 1,       // brouillon, modifiable
    Scheduled = 2,   // planifiée
    Running = 3,     // en cours d'envoi
    Completed = 4,   // terminée
    Cancelled = 5,   // annulée
    Failed = 6       // échec global
}

public enum DeliveryStatus
{
    Pending = 1,     // en attente d'envoi
    Sending = 2,     // pris en charge par un worker
    Sent = 3,        // accepté par le fournisseur
    Delivered = 4,   // confirmé livré (accusé de livraison)
    Failed = 5,      // échec définitif
    Skipped = 6      // ignoré (contact désinscrit, adresse manquante...)
}