using Microsoft.EntityFrameworkCore;
using MiniMessenger.Api.Models;

namespace MiniMessenger.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ===== Messagerie (existant) =====
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Username).HasMaxLength(50).IsRequired();
            entity.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
            entity.HasIndex(u => u.Username).IsUnique();
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.Property(m => m.Content).HasMaxLength(2000).IsRequired();

            entity.HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== Module campagnes =====
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.Property(c => c.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(c => c.LastName).HasMaxLength(100);
            entity.Property(c => c.PhoneNumber).HasMaxLength(20);
            entity.Property(c => c.Email).HasMaxLength(254);

            entity.HasOne(c => c.Owner)
                .WithMany()
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Pas de doublon par propriétaire (les valeurs NULL sont autorisées en plusieurs exemplaires)
            entity.HasIndex(c => new { c.OwnerId, c.PhoneNumber }).IsUnique();
            entity.HasIndex(c => new { c.OwnerId, c.Email }).IsUnique();
        });

        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.Property(c => c.Name).HasMaxLength(150).IsRequired();
            entity.Property(c => c.Subject).HasMaxLength(200);
            entity.Property(c => c.Template).HasMaxLength(2000).IsRequired();

            entity.Property(c => c.Channel).HasConversion<string>().HasMaxLength(20);
            entity.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);

            entity.HasOne(c => c.Owner)
                .WithMany()
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(c => new { c.OwnerId, c.Status });
        });

        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.Property(d => d.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(d => d.LastError).HasMaxLength(500);
            entity.Property(d => d.ProviderMessageId).HasMaxLength(100);

            entity.HasOne(d => d.Campaign)
                .WithMany(c => c.Deliveries)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Contact)
                .WithMany()
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            // Un contact ne reçoit qu'un seul message par campagne
            entity.HasIndex(d => new { d.CampaignId, d.ContactId }).IsUnique();

            // Requête principale du worker : les "Pending" d'une campagne
            entity.HasIndex(d => new { d.CampaignId, d.Status });

            // Retrouver une livraison à partir de l'identifiant du fournisseur (webhooks)
            entity.HasIndex(d => d.ProviderMessageId);
        });
    }
}