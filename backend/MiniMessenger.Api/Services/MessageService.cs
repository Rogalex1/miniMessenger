using Microsoft.EntityFrameworkCore;
using MiniMessenger.Api.Data;
using MiniMessenger.Api.DTOs;
using MiniMessenger.Api.Models;
using MiniMessenger.Api.Services.Interfaces;

namespace MiniMessenger.Api.Services;

public class MessageService : IMessageService
{
    private readonly AppDbContext _db;

    public MessageService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<MessageDto>> GetConversationAsync(int currentUserId, int otherUserId)
    {
        return await _db.Messages
            .Where(m => (m.SenderId == currentUserId && m.ReceiverId == otherUserId)
                     || (m.SenderId == otherUserId && m.ReceiverId == currentUserId))
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                Content = m.Content,
                SentAt = m.SentAt,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId
            })
            .ToListAsync();
    }

    public async Task<MessageDto?> SendAsync(int senderId, SendMessageDto dto)
    {
        if (dto.ReceiverId == senderId) return null;

        var receiverExists = await _db.Users.AnyAsync(u => u.Id == dto.ReceiverId);
        if (!receiverExists) return null;

        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            Content = dto.Content.Trim()
        };

        _db.Messages.Add(message);
        await _db.SaveChangesAsync();

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