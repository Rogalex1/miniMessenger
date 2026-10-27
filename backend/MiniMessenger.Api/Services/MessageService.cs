// using Microsoft.EntityFrameworkCore;
// using MiniMessenger.Api.Data;
// using MiniMessenger.Api.DTOs;
// using MiniMessenger.Api.Models;
// using MiniMessenger.Api.Services.Interfaces;

// namespace MiniMessenger.Api.Services;

// public class MessageService : IMessageService
// {
//     private readonly AppDbContext _db;

//     public MessageService(AppDbContext db)
//     {
//         _db = db;
//     }

//     public async Task<List<MessageDto>> GetConversationAsync(int currentUserId, int otherUserId)
//     {
//         return await _db.Messages
//             .Where(m => (m.SenderId == currentUserId && m.ReceiverId == otherUserId)
//                      || (m.SenderId == otherUserId && m.ReceiverId == currentUserId))
//             .OrderBy(m => m.SentAt)
//             .Select(m => new MessageDto
//             {
//                 Id = m.Id,
//                 Content = m.Content,
//                 SentAt = m.SentAt,
//                 SenderId = m.SenderId,
//                 ReceiverId = m.ReceiverId
//             })
//             .ToListAsync();
//     }

//     public async Task<MessageDto?> SendAsync(int senderId, SendMessageDto dto)
//     {
//         if (dto.ReceiverId == senderId) return null;

//         var receiverExists = await _db.Users.AnyAsync(u => u.Id == dto.ReceiverId);
//         if (!receiverExists) return null;

//         var message = new Message
//         {
//             SenderId = senderId,
//             ReceiverId = dto.ReceiverId,
//             Content = dto.Content.Trim()
//         };

//         _db.Messages.Add(message);
//         await _db.SaveChangesAsync();

//         return new MessageDto
//         {
//             Id = message.Id,
//             Content = message.Content,
//             SentAt = message.SentAt,
//             SenderId = message.SenderId,
//             ReceiverId = message.ReceiverId
//         };
//     }
// }




using MongoDB.Driver;
using MiniMessenger.Api.DTOs;
using MiniMessenger.Api.Models;
using MiniMessenger.Api.Services.Interfaces;

namespace MiniMessenger.Api.Services;

public class MessageService : IMessageService
{
    private readonly IMongoCollection<Message> _messagesCollection;
    private readonly IMongoCollection<User> _usersCollection;

    public MessageService(IMongoDatabase database)
    {
        _messagesCollection = database.GetCollection<Message>("Messages");
        _usersCollection = database.GetCollection<User>("Users");
    }

    public async Task<List<MessageDto>> GetConversationAsync(string currentUserId, string otherUserId)
    {
        // Récupère les messages entre les deux utilisateurs (dans les deux sens)
        var messages = await _messagesCollection
            .Find(m => (m.SenderId == currentUserId && m.ReceiverId == otherUserId)
                    || (m.SenderId == otherUserId && m.ReceiverId == currentUserId))
            .SortBy(m => m.SentAt)
            .ToListAsync();

        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            Content = m.Content,
            SentAt = m.SentAt,
            SenderId = m.SenderId,
            ReceiverId = m.ReceiverId
        }).ToList();
    }

    public async Task<MessageDto?> SendAsync(string senderId, SendMessageDto dto)
    {
        if (dto.ReceiverId == senderId) return null;

        // Vérifie si le destinataire existe dans MongoDB
        var receiverExists = await _usersCollection
            .Find(u => u.Id == dto.ReceiverId)
            .AnyAsync();
            
        if (!receiverExists) return null;

        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            Content = dto.Content.Trim(),
            SentAt = DateTime.UtcNow
        };

        // Insertion dans MongoDB (génère automatiquement l'ObjectId)
        await _messagesCollection.InsertOneAsync(message);

        return new MessageDto
        {
            Id = message.Id,
            Content = message.Content,
            SentAt = message.SentAt,
            SenderId = message.SenderId,
            ReceiverId = message.ReceiverId
        };
    }
}